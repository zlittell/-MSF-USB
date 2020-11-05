using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSF.USBMessages.Tests
{
    [TestClass]
    public class SimpleUSBDatagramTest
    {
        public class SimpleUSBDatagramUnderTest : SimpleUSBMessage
        {
            public void TestNonPublicPropertySetter<T>(string propertyName, T propertyValue)
            {
                this.GetType().GetProperty(propertyName).SetValue(this, propertyValue, null);
            }
        }

        [TestInitialize]
        public void SetUp()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void TestSimpleUSBDatagramShouldSetAndGetPayloadWhenImplemented()
        {
            // Arrange
            ImmutableArray<byte> payload = ImmutableArray.Create(new byte[] { 0x00, 0x01, 0x02 });
            SimpleUSBDatagramUnderTest simpleUSBDatagram = new SimpleUSBDatagramUnderTest();

            // Act
            simpleUSBDatagram.TestNonPublicPropertySetter(nameof(simpleUSBDatagram.Payload), payload);

            // Assert
            Assert.AreEqual(simpleUSBDatagram.Payload, payload);
        }
    }
}
