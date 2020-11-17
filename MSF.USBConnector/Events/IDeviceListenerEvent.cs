// <copyright file="IDeviceListenerEvent.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using Device.Net;

namespace MSF.USBConnector.Events
{
    /// <summary>Describes an event from the Device Listener.</summary>
    public interface IDeviceListenerEvent
    {
        /// <summary>Gets the device event arguments.</summary>
        DeviceEventArgs EventArgs { get; }

        /// <summary>Gets the sender of the event.</summary>
        object Sender { get; }
    }
}