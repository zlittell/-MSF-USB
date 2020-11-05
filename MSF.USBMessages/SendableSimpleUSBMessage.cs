// <copyright file="SendableSimpleUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System;
using System.Collections.Immutable;
using System.Linq;

namespace MSF.USBMessages
{
    /// <summary>Sendable simple USB datagram.</summary>
    public abstract class SendableSimpleUSBMessage : SimpleUSBMessage, ISendableUSBMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendableSimpleUSBMessage"/> class.
        /// </summary>
        /// <param name="payload">Payload used to create the sendable USB datagram.</param>
        /// <exception cref="ArgumentNullException">Payload is null.</exception>
        public SendableSimpleUSBMessage(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            this.Payload = payload.Length == 0 ?
                ImmutableArray.ToImmutableArray(Array.Empty<byte>()) : ImmutableArray.ToImmutableArray(payload);
        }

        /// <inheritdoc/>
        public byte[] ToBytes()
        {
            int headerLength = 0;
            byte[] datagram = new byte[headerLength + this.Payload.Length];

            Buffer.BlockCopy(this.Payload.ToArray(), 0, datagram, headerLength, this.Payload.Length);
            return datagram;
        }
    }
}
