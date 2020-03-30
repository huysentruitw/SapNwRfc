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
        public void Constructor_MessageOnly_ShouldSetMessageAndSetResultCodeToUnknownError()
        {
            // Act
            var exception = new SapException("Some message");

            // Assert
            exception.Message.Should().Be("SAP RFC Error with message: Some message");
            exception.ResultCode.Should().Be(RfcResultCode.RFC_UNKNOWN_ERROR);
        }

        [Fact]
        public void Constructor_NullMessage_ShouldSetFixedMessageAndSetResultCodeToUnknownError()
        {
            // Act
            var exception = new SapException(null);

            // Assert
            exception.Message.Should().Be("SAP RFC Error");
            exception.ResultCode.Should().Be(RfcResultCode.RFC_UNKNOWN_ERROR);
        }

        [Fact]
        public void Constructor_MessageAndCode_ShouldSetMessageAndSetResultCode()
        {
            // Act
            var exception = new SapException(RfcResultCode.RFC_NOT_FOUND, "Some message");

            // Assert
            exception.Message.Should().Be("SAP RFC Error: RFC_NOT_FOUND with message: Some message");
            exception.ResultCode.Should().Be(RfcResultCode.RFC_NOT_FOUND);
        }

        [Fact]
        public void Constructor_NullMessageAndCode_ShouldSetFixedMessageAndSetResultCode()
        {
            // Act
            var exception = new SapException(RfcResultCode.RFC_CANCELED, null);

            // Assert
            exception.Message.Should().Be("SAP RFC Error: RFC_CANCELED");
            exception.ResultCode.Should().Be(RfcResultCode.RFC_CANCELED);
        }
    }
}
