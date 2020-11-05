// <copyright file="IUSBDeviceConnector.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Device.Net;
using MSF.USBMessages;

namespace MSF.USBConnector
{
    /// <summary>
    /// Interface for a USB Device Connector.
    /// </summary>
    public interface IUSBDeviceConnector
    {
        /// <summary>Gets list of device filters.</summary>
        List<FilterDeviceDefinition> DeviceFilters { get; }

        /// <summary>Gets the system listener for device connects/disconnects.</summary>
        public DeviceListener DeviceListener { get; }

        /// <summary>Gets the ILogger registered to the USB functionality.</summary>
        ILogger DeviceLogger { get; }

        /// <summary>Gets the ITracer registered to the USB functionality.</summary>
        ITracer DeviceTracer { get; }

        /// <summary>Gets or sets the USB Device that has been selected for use in functions.</summary>
        IDevice SelectedUSBDevice { get; set; }

        /// <summary>Gets list of connected USB Devices.</summary>
        List<IDevice> USBDeviceList { get; }

        /// <summary>
        /// Function to add a VID and PID combo to the HID Device Filter List.
        /// </summary>
        /// <param name="queryVendorID">USB VID to add.</param>
        /// <param name="queryProductID">USB PID to add.</param>
        void AddHidDeviceToFilterList(uint queryVendorID, uint? queryProductID);

        /// <summary>Closes the open selected USB Device.</summary>
        void CloseUSBDevice();

        /// <summary>Sets the first USB HID devices that matches the filter list as the selected USB Device.</summary>
        void FindFirstMatchingUSBHIDDevice();

        /// <summary>
        /// Opens the selected USB Device.
        /// </summary>
        /// <returns>Awaitable task for this operation.</returns>
        Task OpenUSBDevice();

        /// <summary>
        /// Function to generically parse a payload.
        /// </summary>
        /// <param name="receivedData">ReadResult data received from USB device.</param>
        void ParsePayload(ReadResult receivedData);

        /// <summary>
        /// Updates and refreshes the USB HID Device list based on filter list.
        /// </summary>
        /// <returns>Awaitable task for this operation.</returns>
        Task UpdateUSBHIDDeviceList();

        /// <summary>
        /// Async function for reading data from a USB device.
        /// </summary>
        /// <returns>Awaitable task for this operation.</returns>
        Task ReadUSBDevice();

        /// <summary>
        /// Writes data to the open usb device.
        /// </summary>
        /// <param name="sendData">Byte array to send to device.</param>
        /// <returns>Awaitable task for this operation.</returns>
        Task WriteUSBDevice(byte[] sendData);

        /// <summary>
        /// Writes to the usb devices and then waits for it to return data.
        /// </summary>
        /// <param name="writeData">Byte array of data to write.</param>
        /// <returns>Awaitable task for this operation.</returns>
        Task WriteAndReadUSBDevice(byte[] writeData);

        /// <summary>
        /// Select a device from the IDevice list to use for connections.
        /// </summary>
        /// <param name="selectDevice">Device to select.</param>
        void SelectDevice(IDevice selectDevice);

        /// <summary>
        /// Continously reads from device when detected and selected.
        /// </summary>
        /// <returns>Task for the continous reading.</returns>
        Task ContinousRead();

        /// <summary>
        /// Send a USB Message to connected device.
        /// </summary>
        /// <param name="messageToSend">ISendableUSBMessage to send to device.</param>
        /// <returns>Task for write operation.</returns>
        Task SendUSBMessage(ISendableUSBMessage messageToSend);

        /// <summary>
        /// Handles received messages from USB Devices.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        void ReceivedUSBMessageHandler(IReceivableUSBMessage message);
    }
}