using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace MSF.USBConnector.Tests
{
  [TestClass]
  public class USBDeviceConnectorDUTTest
  {
    [TestMethod]
    public void TestShouldUpdateUSBHIDDeviceList()
    {
      // Arrange
      USBDeviceConnector myDeviceConnector = new USBDeviceConnector(new Mock<IEventAggregator>().Object);

      // Act
      myDeviceConnector.RefreshFilteredDeviceList();

      // Assert
      Assert.AreNotEqual(0, myDeviceConnector.USBConnectedDeviceList.Count);
    }

    [TestMethod]
    public void TestShouldSelectAUSBHIDDeviceFromList()
    {
      // Arrange
      USBDeviceConnector myDeviceConnector = new USBDeviceConnector(new Mock<IEventAggregator>().Object);

      // Act
      myDeviceConnector.RefreshFilteredDeviceList();

      // Assert
      Assert.IsNotNull(myDeviceConnector.SelectedUSBDevice);
    }
  }
}