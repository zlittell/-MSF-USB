using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSF.USBMessages.Tests
{
    [TestClass]
    public class ReceivableSimpleUSBDatagramTest
    {
        public class ReceivableSimpleUSBDatagramUnderTest : ReceivableSimpleUSBMessage
        {
            public ImmutableArray<byte> ParsedPayload { get; private set; }

            public override void ParsePayload(ImmutableArray<byte> payload)
            {
                this.ParsedPayload = payload;
            }
        }

        [TestInitialize]
        public void SetUp()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void TestReceivableSimpleUSBDatagramShouldCallParsePayload()
        {
            // Arrange
            byte[] payload = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

            // Act
            ReceivableSimpleUSBDatagramUnderTest receivableSimpleUSBDatagramUnderTest =
                ReceivableSimpleUSBMessage.FromBytes<ReceivableSimpleUSBDatagramUnderTest>(payload);

            Assert.AreEqual(receivableSimpleUSBDatagramUnderTest.ParsedPayload, receivableSimpleUSBDatagramUnderTest.Payload);
        }
    }
}
