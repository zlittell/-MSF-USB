// <copyright file="ISendableUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

namespace MSF.USBMessages
{
    /// <summary>Sendable USB datagram interface.</summary>
    public interface ISendableUSBMessage : ISimpleUSBMessage
    {
        /// <summary>
        /// To bytes.
        /// </summary>
        /// <returns>Sendable USB datagram as bytes.</returns>
        byte[] ToBytes();
    }
}
