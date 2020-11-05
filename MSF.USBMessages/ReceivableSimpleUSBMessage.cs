// <copyright file="ReceivableSimpleUSBMessage.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System;
using System.Collections.Immutable;
using System.Linq;

namespace MSF.USBMessages
{
    /// <summary>Receivable simple USB datagram.</summary>
    public class ReceivableSimpleUSBMessage : SimpleUSBMessage, IReceivableUSBMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivableSimpleUSBMessage"/> class.
        /// </summary>
        protected ReceivableSimpleUSBMessage()
        {
        }

        /// <summary>
        /// From bytes.
        /// </summary>
        /// <typeparam name="T">Type of receivable USB datagram.</typeparam>
        /// <param name="data">Data used to create the receivable USB datagram of type T.</param>
        /// <returns>Instance of receivable simple USB message of type T.</returns>
        /// <exception cref="ArgumentNullException">Data is null.</exception>
        public static T FromBytes<T>(byte[] data)
            where T : ReceivableSimpleUSBMessage
        {
            ReceivableSimpleUSBMessage result = Activator.CreateInstance(typeof(T)) as ReceivableSimpleUSBMessage;

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            int headerLength = 0;
            byte[] dataWithoutHeader = data.Skip(headerLength).Take(data.Length - headerLength).ToArray();
            result.Payload = ImmutableArray.ToImmutableArray(dataWithoutHeader.ToArray());
            result.ParsePayload(result.Payload);
            return (T)result;
        }

        /// <inheritdoc/>
        public virtual void ParsePayload(ImmutableArray<byte> payload)
        {
        }
    }
}
