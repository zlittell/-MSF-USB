// <copyright file="USBDeviceConnector.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Device.Net;
using Hid.Net.Windows;
using MSF.USBConnector.Events;
using MSF.USBMessages;

namespace MSF.USBConnector
{
  /// <summary>Class for usb device connection.</summary>
  public class USBDeviceConnector
  {
    /// <summary>Constant millisecond time for polling frequency of device arrival/removal.</summary>
    private const int PollMilliseconds = 3000;

    /// <summary>
    /// Initializes a new instance of the <see cref="USBDeviceConnector"/> class.
    /// </summary>
    /// <param name="aggregator">Event Aggregator wired in from caliburn micro.</param>
    public USBDeviceConnector(IEventAggregator aggregator)
    {
      this.EventAggregator = aggregator;
      this.EventAggregator.SubscribeOnBackgroundThread(this);
      WindowsHidDeviceFactory.Register(this.DeviceLogger, this.DeviceTracer);
      this.SetupDeviceListener();
    }

    /// <summary>Gets or sets the USB Device that has been selected for use in functions.</summary>
    public virtual IDevice SelectedUSBDevice { get; set; }

    /// <summary>Gets list of connected USB Devices.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Order, single elements, and removeall")]
    public List<IDevice> USBDeviceList { get; private set; } = new List<IDevice>();

    /// <summary>Gets or sets interface for filtering devicelist.</summary>
    protected virtual string UsbInterface { get; set; } = string.Empty;

    /// <summary>Gets list of device filters.</summary>
    protected virtual Collection<FilterDeviceDefinition> DeviceFilters { get; } = new Collection<FilterDeviceDefinition>();

    /// <summary>Gets Caliburn Micro Event Aggregator.</summary>
    protected IEventAggregator EventAggregator { get; }

    /// <summary>Gets the cancellation token for USB continous read task.</summary>
    protected CancellationToken ContinousReadCancellationToken { get; }

    /// <summary>Gets or sets the system listener for device connects/disconnects.</summary>
    private DeviceListener DeviceListener { get; set; }

    /// <summary>Gets the ILogger registered to the USB functionality.</summary>
    private ILogger DeviceLogger { get; } = new DebugLogger();

    /// <summary>Gets the ITracer registered to the USB functionality.</summary>
    private ITracer DeviceTracer { get; } = new DebugTracer();

    /// <summary>
    /// Send a USB Message to connected device.
    /// </summary>
    /// <param name="messageToSend">ISendableUSBMessage to send to device.</param>
    /// <returns>Task for write operation.</returns>
    public async Task SendUSBMessage(ISendableUSBMessage messageToSend)
    {
      if (messageToSend != null)
      {
        await this.WriteUSBDevice(messageToSend.ToBytes()).ConfigureAwait(true);
      }
      else
      {
        throw new ArgumentException("Message to send cannot be null.", nameof(messageToSend));
      }
    }

    /// <summary>Refresh device list and filter it for only this devices correct interface.</summary>
    public virtual void RefreshFilteredDeviceList()
    {
      Task.Run(async () => { await this.UpdateUSBHIDDeviceList().ConfigureAwait(true); }).Wait();
      this.USBDeviceList.RemoveAll(this.DoesNotContainCorrectInterface);

      if (this.USBDeviceList.Count > 0)
      {
        if (this.SelectedUSBDevice == null | !this.DoesDeviceListContainDevice(this.SelectedUSBDevice))
        {
          this.SelectDevice(this.USBDeviceList.First());
        }
      }
      else
      {
        this.SelectDevice(null);
      }
    }

    /// <summary>
    /// This method should be ran after the derived class is initialized.
    /// </summary>
    protected void RunAfterInitialized()
    {
      Task.Run(() => this.ContinousRead(), this.ContinousReadCancellationToken);
      this.RefreshFilteredDeviceList();
    }

    /// <summary>Sets up Device Listener.</summary>
    protected void SetupDeviceListener()
    {
      this.DeviceListener?.Dispose();
      this.DeviceListener = new DeviceListener(this.DeviceFilters, PollMilliseconds) { Logger = this.DeviceLogger };
      this.DeviceListener.DeviceDisconnected += this.DeviceDisconnectEvent;
      this.DeviceListener.DeviceInitialized += this.DeviceConnectedEvent;
      this.DeviceListener.Start();
    }

    /// <summary>
    /// Function to add a VID and PID combo to the HID Device Filter List.
    /// </summary>
    /// <param name="filterDevice">Filter definiteion to add.</param>
    protected void AddHidDeviceToFilterList(FilterDeviceDefinition filterDevice)
    {
      this.DeviceFilters.Add(filterDevice);
    }

    /// <summary>
    /// Updates and refreshes the USB HID Device list based on filter list.
    /// </summary>
    /// <returns>Awaitable task for this operation.</returns>
    protected async Task UpdateUSBHIDDeviceList()
    {
      this.USBDeviceList = await DeviceManager.Current.GetDevicesAsync(this.DeviceFilters.ToList<FilterDeviceDefinition>()).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens the selected USB Device.
    /// </summary>
    /// <returns>Awaitable task for this operation.</returns>
    protected async Task OpenUSBDevice()
    {
      if (this.SelectedUSBDevice != null)
      {
        await this.SelectedUSBDevice.InitializeAsync().ConfigureAwait(false);
      }
    }

    /// <summary>Closes the open selected USB Device.</summary>
    protected void CloseUSBDevice()
    {
      if (this.SelectedUSBDevice != null)
      {
        this.SelectedUSBDevice.Close();
      }
    }

    /// <summary>
    /// Function to generically parse a payload.
    /// </summary>
    /// <param name="receivedData">ReadResult data received from USB device.</param>
    protected virtual void ParsePayload(ReadResult receivedData)
    {
      ReceivableSimpleUSBMessage message = ReceivableSimpleUSBMessage.FromBytes<ReceivableSimpleUSBMessage>(receivedData);
      this.ReceivedUSBMessageHandler(message);
    }

    /// <summary>
    /// Handles received messages from USB Devices.
    /// </summary>
    /// <param name="message">Message to handle.</param>
    protected virtual void ReceivedUSBMessageHandler(IReceivableUSBMessage message)
    {
      this.EventAggregator.PublishOnBackgroundThreadAsync(message);
    }

    /// <summary>
    /// Writes to the usb devices and then waits for it to return data.
    /// </summary>
    /// <param name="writeData">Byte array of data to write.</param>
    /// <returns>Awaitable task for this operation.</returns>
    protected async Task WriteAndReadUSBDevice(byte[] writeData)
    {
      ReadResult readBytes = await this.SelectedUSBDevice.WriteAndReadAsync(writeData).ConfigureAwait(false);
      this.ParsePayload(readBytes);
    }

    /// <summary>
    /// Async function for reading data from a USB device.
    /// </summary>
    /// <returns>Awaitable task for this operation.</returns>
    protected async Task ReadUSBDevice()
    {
      if ((this.SelectedUSBDevice != null) && this.SelectedUSBDevice.IsInitialized)
      {
        ReadResult readBytes = await this.SelectedUSBDevice.ReadAsync().ConfigureAwait(false);
        this.ParsePayload(readBytes);
      }
    }

    /// <summary>
    /// Writes data to the open usb device.
    /// </summary>
    /// <param name="sendData">Byte array to send to device.</param>
    /// <returns>Awaitable task for this operation.</returns>
    protected async Task WriteUSBDevice(byte[] sendData)
    {
      if (sendData != null)
      {
        if (this.SelectedUSBDevice != null)
        {
          if (sendData.Length <= this.SelectedUSBDevice.ConnectedDeviceDefinition?.WriteBufferSize)
          {
            byte[] data = new byte[(int)this.SelectedUSBDevice.ConnectedDeviceDefinition.WriteBufferSize];
            Buffer.BlockCopy(sendData, 0, data, 0, sendData.Length);
            await this.SelectedUSBDevice.WriteAsync(data).ConfigureAwait(true);
          }
          else
          {
            throw new ArgumentOutOfRangeException(nameof(sendData), "Cannot write data larger than buffer");
          }
        }
        else
        {
          throw new ArgumentNullException(nameof(this.SelectedUSBDevice), "Trying to write but a device is not selected.");
        }
      }
      else
      {
        {
          throw new ArgumentNullException(nameof(sendData), "Data to write to device cannot be null");
        }
      }
    }

    /// <summary>
    /// Continously reads from device when detected and selected.
    /// </summary>
    /// <returns>Task for the continous reading.</returns>
    protected async Task ContinousRead()
    {
      while (true)
      {
        if (this.SelectedUSBDevice != null)
        {
          try
          {
            await this.ReadUSBDevice().ConfigureAwait(false);
          }
          catch (IOException ex) when (ex.InnerException?.Message == "The device is not connected.")
          {
            this.SelectDevice(null);
          }
          catch (Exception ex) when (ex.Message == "The device has not been initialized.")
          {
            // debug write this? seems not useful
          }
          catch (Exception ex)
          {
            Debug.WriteLine(ex.Message.ToString());
            throw new IOException("Problem in continous read thread.", ex);
          }
        }
      }
    }

    /// <summary>
    /// Select a device from the IDevice list to use for connections.
    /// </summary>
    /// <param name="selectDevice">Device to select.</param>
    protected void SelectDevice(IDevice selectDevice)
    {
      this.SelectedUSBDevice = selectDevice;
    }

    /// <summary>
    /// Checks if the device list contains a device (Compares DeviceIDs).
    /// </summary>
    /// <param name="device">Device to check for.</param>
    /// <returns>True if exists in list.</returns>
    private bool DoesDeviceListContainDevice(IDevice device)
    {
      foreach (IDevice checkDevice in this.USBDeviceList)
      {
        if (checkDevice.DeviceId == device?.DeviceId)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Event Called when filtered device connects.
    /// </summary>
    /// <param name="sender">Event Sender.</param>
    /// <param name="e">Event Arguments.</param>
    private void DeviceConnectedEvent(object sender, DeviceEventArgs e)
    {
      this.EventAggregator.PublishOnUIThreadAsync(new DeviceConnectedEvent(sender, e));
    }

    /// <summary>
    /// Event Called when filtered device connects.
    /// </summary>
    /// <param name="sender">Event Sender.</param>
    /// <param name="e">Event Arguments.</param>
    private void DeviceDisconnectEvent(object sender, DeviceEventArgs e)
    {
      this.EventAggregator.PublishOnUIThreadAsync(new DeviceDisconnectedEvent(sender, e));
    }

    /// <summary>
    /// Returns if a device is the wrong interface.
    /// </summary>
    /// <param name="obj">IDevice to check interface.</param>
    /// <returns>True if does not contain correct interface.</returns>
    private bool DoesNotContainCorrectInterface(IDevice obj)
    {
      return !obj.DeviceId.ContainsIgnoreCase(this.UsbInterface);
    }
  }
}
