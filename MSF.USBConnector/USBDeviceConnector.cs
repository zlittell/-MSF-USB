// <copyright file="USBDeviceConnector.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
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
    /// <summary>Gets or sets interface for filtering devicelist.</summary>
    protected virtual string UsbInterface { get; set; } = string.Empty;

    /// <summary>Gets or sets Device Definition for filtering devices.</summary>
    protected virtual FilterDeviceDefinition DeviceDefinition { get; set; } = new FilterDeviceDefinition();

    protected IEventAggregator EventAggregator { get; }

    protected CancellationToken continousReadCancellationToken { get; set; }

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
      this.AddHidDeviceToFilterList(this.DeviceDefinition);
      this.SetupDeviceListener();
    }

    /// <summary>
    /// This method should be ran after the derived class is initialized.
    /// </summary>
    protected void RunAfterInitialized()
    {
      Task.Run(() => this.ContinousRead(), this.continousReadCancellationToken);
      this.RefreshFilteredDeviceList();
    }

    /// <inheritdoc/>
    public DeviceListener DeviceListener { get; private set; }

    /// <inheritdoc/>
    public ILogger DeviceLogger { get; } = new DebugLogger();

    /// <inheritdoc/>
    public ITracer DeviceTracer { get; } = new DebugTracer();

        /// <inheritdoc/>
    public virtual IDevice SelectedUSBDevice { get; set; }

    /// <inheritdoc/>
    public List<IDevice> USBDeviceList { get; private set; } = new List<IDevice>();

    /// <inheritdoc/>
    public List<FilterDeviceDefinition> DeviceFilters { get; private set; } = new List<FilterDeviceDefinition>();

    /// <summary>Sets up Device Listener.</summary>
    public void SetupDeviceListener()
    {
      this.DeviceListener?.Dispose();
      this.DeviceListener = new DeviceListener(this.DeviceFilters, PollMilliseconds) { Logger = this.DeviceLogger };
      this.DeviceListener.DeviceDisconnected += this.DeviceDisconnectEvent;
      this.DeviceListener.DeviceInitialized += this.DeviceConnectedEvent;
      this.DeviceListener.Start();
    }

    /// <inheritdoc/>
    public void AddHidDeviceToFilterList(FilterDeviceDefinition filterDevice)
    {
      this.DeviceFilters.Add(filterDevice);
    }

    /// <inheritdoc/>
    public async Task UpdateUSBHIDDeviceList()
    {
      try
      {
        this.USBDeviceList = await DeviceManager.Current.GetDevicesAsync(this.DeviceFilters).ConfigureAwait(false);
      }
      catch (Exception)
      {
        throw new Exception($"Failed to retrieve filtered list of USB HID Devices that match the filtered list.");
      }
    }

    /// <inheritdoc/>
    public void FindFirstMatchingUSBHIDDevice()
    {
      this.SelectedUSBDevice = this.USBDeviceList.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task OpenUSBDevice()
    {
      try
      {
        if (this.SelectedUSBDevice != null)
        {
          await this.SelectedUSBDevice.InitializeAsync().ConfigureAwait(false);
        }
      }
      catch (ArgumentNullException)
      {
        throw new Exception("USB Device to open was null. Select and assign a device to class.");
      }
    }

    /// <inheritdoc/>
    public void CloseUSBDevice()
    {
      try
      {
        if (this.SelectedUSBDevice != null)
        {
          this.SelectedUSBDevice.Close();
        }
      }
      catch (ArgumentNullException)
      {
        throw new Exception("Tried to close a USB Device Stream, but the stream was null. Can only close a stream that is opened.");
      }
    }

    /// <inheritdoc/>
    public virtual void ParsePayload(ReadResult receivedData)
    {
      ReceivableSimpleUSBMessage message = ReceivableSimpleUSBMessage.FromBytes<ReceivableSimpleUSBMessage>(receivedData);
      this.ReceivedUSBMessageHandler(message);
    }

    /// <inheritdoc/>
    public virtual void ReceivedUSBMessageHandler(IReceivableUSBMessage message)
    {
      this.EventAggregator.PublishOnBackgroundThreadAsync(message);
    }

    /// <inheritdoc/>
    public async Task WriteAndReadUSBDevice(byte[] writeData)
    {
      ReadResult readBytes = await this.SelectedUSBDevice.WriteAndReadAsync(writeData).ConfigureAwait(false);
      this.ParsePayload(readBytes);
    }

    /// <inheritdoc/>
    public async Task ReadUSBDevice()
    {
      if ((this.SelectedUSBDevice != null) && this.SelectedUSBDevice.IsInitialized)
      {
        ReadResult readBytes = await this.SelectedUSBDevice.ReadAsync().ConfigureAwait(false);
        this.ParsePayload(readBytes);
      }
      else
      {
        throw new Exception("Selected device is null or not initialized.");
      }
    }

    /// <inheritdoc/>
    public async Task WriteUSBDevice(byte[] sendData)
    {
      if (sendData != null)
      {
        if (this.SelectedUSBDevice != null)
        {
          if (sendData.Length <= this.SelectedUSBDevice.ConnectedDeviceDefinition?.WriteBufferSize)
          {
            byte[] data = new byte[(int)this.SelectedUSBDevice.ConnectedDeviceDefinition.WriteBufferSize];
            Buffer.BlockCopy(sendData, 0, data, 0, sendData.Length);
            try
            {
              await this.SelectedUSBDevice.WriteAsync(data).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
              throw new Exception("Error trying to write to device.", ex);
            }
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

    /// <inheritdoc/>
    public async Task ContinousRead()
    {
      while (true)
      {
        if (this.SelectedUSBDevice != null)
        {
          if (this.SelectedUSBDevice.IsInitialized)
          {
            try
            {
              await this.ReadUSBDevice().ConfigureAwait(false);
            }
            catch (System.IO.IOException ex) when (ex.InnerException?.Message == "The device is not connected.")
            {
              this.SelectDevice(null);
            }
            catch (System.Exception ex) when (ex.Message == "The device has not been initialized.")
            {
              System.Diagnostics.Debug.WriteLine("Device isn't initialized for reading yet");
            }
            catch (Exception ex)
            {
              throw new Exception("Exception occured when trying to continously read a usb device.", ex);
            }
          }
        }
      }
    }

    /// <inheritdoc/>
    public async Task SendUSBMessage(ISendableUSBMessage messageToSend)
    {
      if (messageToSend != null)
      {
        var bytes = messageToSend.ToBytes();
        await this.WriteUSBDevice(messageToSend.ToBytes()).ConfigureAwait(true);
      }
      else
      {
        throw new ArgumentException("Message to send cannot be null.", nameof(messageToSend));
      }
    }

    /// <inheritdoc/>
    public void SelectDevice(IDevice selectDevice)
    {
      this.SelectedUSBDevice = selectDevice;
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
    /// Checks if the device list contains a device (Compares DeviceIDs).
    /// </summary>
    /// <param name="device">Device to check for.</param>
    /// <returns>True if exists in list.</returns>
    protected internal bool DoesDeviceListContainDevice(IDevice device)
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
