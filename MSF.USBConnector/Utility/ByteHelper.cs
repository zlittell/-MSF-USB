// <copyright file="ByteHelper.cs" company="Mechanical Squid Factory">
// Copyright © Mechanical Squid Factory All rights reserved.
// </copyright>

using System;

namespace MSF.USBConnector.Utility
{
    /// <summary>Helper class related to bytes from microcontrollers.</summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Converts an integer to a byte array with the MSB first.
        /// </summary>
        /// <param name="toConvert">Integer to convert to bytes.</param>
        /// <returns>Big Endian Byte Array.</returns>
        public static byte[] IntToBigEndian(int toConvert)
        {
            byte[] result = BitConverter.GetBytes(toConvert);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(result);
            }

            return result;
        }

        /// <summary>
        /// Converts an integer to a byte array with the LSB first.
        /// </summary>
        /// <param name="toConvert">Integer to convert to bytes.</param>
        /// <returns>Little Endian Byte Array.</returns>
        public static byte[] IntToLittleEndian(int toConvert)
        {
            byte[] result = BitConverter.GetBytes(toConvert);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(result);
            }

            return result;
        }

        /// <summary>
        /// Converts a byte array to an integer with the MSB first.
        /// </summary>
        /// <param name="toConvert">Array of 4 bytes to make int.</param>
        /// <returns>Integer converted value.</returns>
        public static int ByteArrayToIntBigEndian(byte[] toConvert)
        {
            // User should always provide data according to function name
            // Then we check what BitConverter is expecting and alter it
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(toConvert);
            }

            return BitConverter.ToInt32(toConvert);
        }

        /// <summary>
        /// Converts a byte array to an integer with the LSB first.
        /// </summary>
        /// <param name="toConvert">Array of 4 bytes to make int.</param>
        /// <returns>Integer converted value.</returns>
        public static int ByteArrayToIntLittleEndian(byte[] toConvert)
        {
            // User should always provide data according to function name
            // Then we check what BitConverter is expecting and alter it
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(toConvert);
            }

            return BitConverter.ToInt32(toConvert);
        }
    }
}
