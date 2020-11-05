// <copyright file="ISimpleUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System.Collections.Immutable;

namespace MSF.USBMessages
{
    /// <summary>Simple USB Datagram interface.</summary>
    public interface ISimpleUSBMessage
    {
        /// <summary>Gets payload.</summary>
        ImmutableArray<byte> Payload { get; }
    }
}