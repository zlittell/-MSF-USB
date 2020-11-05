// <copyright file="SimpleUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System.Collections.Immutable;

namespace MSF.USBMessages
{
    /// <summary>
    /// Describes the generic simple USB datagram used for MSF devices.
    /// </summary>
    public class SimpleUSBMessage : ISimpleUSBMessage
    {
        /// <summary>Gets or sets payload array. </summary>
        public ImmutableArray<byte> Payload { get; protected set; }
    }
}
