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

    public class ReceivedSimpleUSBDatagramNoPayloadParseUnderTest : ReceivableSimpleUSBMessage
    {
      public ImmutableArray<byte> ParsedPayload { get; private set; }
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

      // Assert
      Assert.AreEqual(receivableSimpleUSBDatagramUnderTest.ParsedPayload, receivableSimpleUSBDatagramUnderTest.Payload);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Just a simple assignment to throw a null")]
    public void TestReceivableSimpleUSBDatagramShouldThrowExceptionDataNull()
    {
      // Arrange, Act
      ReceivableSimpleUSBDatagramUnderTest receivableSimpleUSBDatagramUnderTest =
          ReceivableSimpleUSBMessage.FromBytes<ReceivableSimpleUSBDatagramUnderTest>(null);
    }

    [TestMethod]
    public void TestReceivableSimpleUSBDatagramParsedPayloadNotProcessedByDefault()
    {
      // Arrange
      byte[] payload = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

      // Act
      ReceivedSimpleUSBDatagramNoPayloadParseUnderTest receivableSimpleUSBMessage =
        ReceivableSimpleUSBMessage.FromBytes<ReceivedSimpleUSBDatagramNoPayloadParseUnderTest>(payload);

      // Assert
      Assert.AreNotEqual(payload, receivableSimpleUSBMessage.ParsedPayload);
    }
  }
}
