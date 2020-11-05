using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSF.USBMessages.Tests
{
    [TestClass]
    public class SendableSimpleUSBDatagramTest
    {
        public sealed class SendableSimpleUSBDatagramUnderTest : SendableSimpleUSBMessage
        {
            public SendableSimpleUSBDatagramUnderTest(byte[] payload)
                : base(payload)
            {
            }
        }

        [TestInitialize]
        public void SetUp()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendableSimpleUSBDatagramShouldThrowNullExceptionWhenPayloadIsNull()
        {
            // Arrange, Act, Assert
            new SendableSimpleUSBDatagramUnderTest(null);
        }

        [TestMethod]
        public void TestSendableSimpleUSBDatagramShouldSetPayloadWhenInitialized()
        {
            // Arrange
            byte[] payload = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            // Act
            SendableSimpleUSBDatagramUnderTest sendableSimpleUSBDatagramUnderTest = new SendableSimpleUSBDatagramUnderTest(payload);

            // Assert
            CollectionAssert.AreEqual(sendableSimpleUSBDatagramUnderTest.Payload, payload);
        }

        [TestMethod]
        public void TestSendableSimpleUSBDatagramShouldConvertToBytesWhenCalled()
        {
            // Arrange
            byte[] payload = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

            SendableSimpleUSBDatagramUnderTest sendableSimpleUSBDatagramUnderTest = new SendableSimpleUSBDatagramUnderTest(payload);

            // Act, Assert
            CollectionAssert.AreEqual(sendableSimpleUSBDatagramUnderTest.ToBytes(), payload);
        }
    }
}
