using System;
using AutoFixture;
using FluentAssertions;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Exceptions
{
    public sealed class SapExceptionTests
    {
        private static readonly Fixture Fixture = new Fixture();

        [Fact]
        public void ShouldInheritFromException()
        {
            // Assert
            typeof(Exception).IsAssignableFrom(typeof(SapException)).Should().BeTrue();
        }

        [Fact]
        public void Constructor_CodeAndErrorInfoWithMessage_ShouldSetMessageAndSetResultCode()
        {
            // Act
            var errorInfo = new RfcErrorInfo { Message = "Some message" };
            var exception = new SapException(RfcResultCode.RFC_NOT_FOUND, errorInfo);

            // Assert
            exception.Message.Should().Be("SAP RFC Error: RFC_NOT_FOUND with message: Some message");
            exception.ResultCode.Should().Be(SapResultCode.NotFound);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_CodeAndErrorInfoWithoutMessage_ShouldSetFixedMessageAndSetResultCode(string message)
        {
            // Act
            var errorInfo = new RfcErrorInfo { Message = message };
            var exception = new SapException(RfcResultCode.RFC_CANCELED, errorInfo);

            // Assert
            exception.Message.Should().Be("SAP RFC Error: RFC_CANCELED");
            exception.ResultCode.Should().Be(SapResultCode.Canceled);
        }

        [Fact]
        public void Constructor_ShouldCopyErrorInfo()
        {
            // Arrange
            var errorInfo = new RfcErrorInfo
            {
                ErrorGroup = RfcErrorGroup.LOGON_FAILURE,
                Key = "Some Key",
                Message = "Some Message",
                AbapMsgClass = "Some AbapMsgClass",
                AbapMsgType = "Some AbapMsgType",
                AbapMsgNumber = "Some AbapMsgNumber",
                AbapMsgV1 = "Some AbapMsgV1",
                AbapMsgV2 = "Some AbapMsgV2",
                AbapMsgV3 = "Some AbapMsgV3",
                AbapMsgV4 = "Some AbapMsgV4",
            };

            // Act
            var exception = new SapException(RfcResultCode.RFC_CLOSED, errorInfo);

            // Assert
            exception.ErrorInfo.ErrorGroup.Should().Be(SapErrorGroup.LogonFailure);
            exception.ErrorInfo.Key.Should().Be(errorInfo.Key);
            exception.ErrorInfo.Message.Should().Be(errorInfo.Message);
            exception.ErrorInfo.AbapMessageClass.Should().Be(errorInfo.AbapMsgClass);
            exception.ErrorInfo.AbapMessageType.Should().Be(errorInfo.AbapMsgType);
            exception.ErrorInfo.AbapMessageNumber.Should().Be(errorInfo.AbapMsgNumber);
            exception.ErrorInfo.AbapMessageV1.Should().Be(errorInfo.AbapMsgV1);
            exception.ErrorInfo.AbapMessageV2.Should().Be(errorInfo.AbapMsgV2);
            exception.ErrorInfo.AbapMessageV3.Should().Be(errorInfo.AbapMsgV3);
            exception.ErrorInfo.AbapMessageV4.Should().Be(errorInfo.AbapMsgV4);
        }
    }
}
