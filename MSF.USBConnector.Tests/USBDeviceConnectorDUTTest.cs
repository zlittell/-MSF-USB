using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MSF.USBConnector.Tests
{
    /*
     * These tests need a usb test device inserted
     */

    [TestClass]
    public class USBDeviceConnectorDUTTest
    {
        [TestMethod]
        public void TestShouldUpdateUSBHIDDeviceList()
        {
            // Arrange
            var eventAggregatorMock = new Mock<IEventAggregator>();
            USBDeviceConnector myDeviceConnector = new USBDeviceConnector(eventAggregatorMock.Object);
            myDeviceConnector.AddHidDeviceToFilterList(0x16C0, 0x0486);

            // Act
            Task.Run(async () => { await myDeviceConnector.UpdateUSBHIDDeviceList().ConfigureAwait(false); }).Wait();

            // Assert
            Assert.AreNotEqual(0, myDeviceConnector.USBDeviceList.Count);
        }

        [TestMethod]
        public void TestShouldSelectAUSBHIDDeviceFromList()
        {
            // Arrange
            var eventAggregatorMock = new Mock<IEventAggregator>();
            USBDeviceConnector myDeviceConnector = new USBDeviceConnector(eventAggregatorMock.Object);
            myDeviceConnector.AddHidDeviceToFilterList(0x16C0, 0x0486);
            Task.Run(async () => { await myDeviceConnector.UpdateUSBHIDDeviceList().ConfigureAwait(false); }).Wait();

            // Act
            myDeviceConnector.FindFirstMatchingUSBHIDDevice();

            // Assert
            Assert.IsNotNull(myDeviceConnector.SelectedUSBDevice);
        }

        [TestMethod]
        public void TestShouldOpenUSBHIDDevice()
        {
            // Arrange
            var eventAggregatorMock = new Mock<IEventAggregator>();
            USBDeviceConnector myDeviceConnector = new USBDeviceConnector(eventAggregatorMock.Object);
            myDeviceConnector.AddHidDeviceToFilterList(0x16C0, 0x0486);
            Task.Run(async () => { await myDeviceConnector.UpdateUSBHIDDeviceList().ConfigureAwait(false); }).Wait();
            myDeviceConnector.FindFirstMatchingUSBHIDDevice();

            // Act
            Task.Run(async () => { await myDeviceConnector.OpenUSBDevice().ConfigureAwait(false); }).Wait();

            // Assert
            Assert.IsTrue(myDeviceConnector.SelectedUSBDevice.IsInitialized);
            myDeviceConnector.CloseUSBDevice();
        }
    }
}