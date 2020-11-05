// <copyright file="DeviceConnectedEvent.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using Device.Net;

namespace MSF.USBConnector.Events
{
    /// <summary>Message For Device Connecting.</summary>
    public class DeviceConnectedEvent : IDeviceListenerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceConnectedEvent"/> class.
        /// </summary>
        /// <param name="s">Sender Object.</param>
        /// <param name="e">DeviceEventArgs.</param>
        internal DeviceConnectedEvent(object s, DeviceEventArgs e)
        {
            this.Sender = s;
            this.EventArgs = e;
        }

        /// <inheritdoc/>
        public object Sender { get; }

        /// <inheritdoc/>
        public DeviceEventArgs EventArgs { get; }
    }
}