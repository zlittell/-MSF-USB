using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSF.USBConnector.Utility;

namespace MSF.USBConnector.Tests
{
    [TestClass]
    public class USBConnectorByteHelperTests
    {
        [TestInitialize]
        public void SetUp()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void TestIntToBigEndianShouldConvertCorrectly()
        {
            int testInteger = 300;
            byte[] correctBytes = { 0x00, 0x00, 0x01, 0x2c };
            byte[] convertedBytes = ByteHelper.IntToBigEndian(testInteger);

            CollectionAssert.AreEqual(correctBytes, convertedBytes);
        }

        [TestMethod]
        public void TestIntToLittleEndianShouldConvertCorrectly()
        {
            int testInteger = 300;
            byte[] correctBytes = { 0x2c, 0x01, 0x00, 0x00 };
            byte[] convertedBytes = ByteHelper.IntToLittleEndian(testInteger);

            CollectionAssert.AreEqual(correctBytes, convertedBytes);
        }

        [TestMethod]
        public void TestByteArrayToIntBigEndianShouldConvertCorrectly()
        {
            byte[] testBytes = { 0x00, 0x00, 0x11, 0x94 };
            int correctInteger = 4500;
            int convertedInteger = ByteHelper.ByteArrayToIntBigEndian(testBytes);

            Assert.AreEqual(correctInteger, convertedInteger);
        }

        [TestMethod]
        public void TestByteArrayToIntLittleEndianShouldConvertCorrectly()
        {
            byte[] testBytes = { 0x89, 0x1c, 0x06, 0x00 };
            int correctInteger = 400521;
            int convertedInteger = ByteHelper.ByteArrayToIntLittleEndian(testBytes);

            Assert.AreEqual(correctInteger, convertedInteger);
        }
    }
}
