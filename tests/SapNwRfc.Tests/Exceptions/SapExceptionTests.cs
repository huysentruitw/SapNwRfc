using System;
using FluentAssertions;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Exceptions
{
    public sealed class SapExceptionTests
    {
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
    }
}
