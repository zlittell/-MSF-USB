// <copyright file="IReceivableUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System.Collections.Immutable;

namespace MSF.USBMessages
{
    /// <summary>Receivable USB datagram interface.</summary>
    public interface IReceivableUSBMessage : ISimpleUSBMessage
    {
        /// <summary>
        /// Parse payload.
        /// </summary>
        /// <param name="payload">Payload to parse.</param>
        void ParsePayload(ImmutableArray<byte> payload);
    }
}
