using System;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal.Interop.Extensions
{
    public sealed class RfcResultCodeExtensionsTests
    {
        [Fact]
        public void ThrowOnError_NoError_ShouldNotThrow()
        {
            // Arrange
            RfcResultCode resultCode = RfcResultCode.RFC_OK;
            var errorInfo = default(RfcErrorInfo);

            // Act
            Action action = () => resultCode.ThrowOnError(errorInfo);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowOnError_NoError_ShouldNotCallBeforeThrowAction()
        {
            // Arrange
            RfcResultCode resultCode = RfcResultCode.RFC_OK;
            var errorInfo = default(RfcErrorInfo);
            var beforeThrowActionMock = new Mock<Action>();

            // Act
            resultCode.ThrowOnError(errorInfo, beforeThrowActionMock.Object);

            // Assert
            beforeThrowActionMock.Verify(x => x(), Times.Never);
        }

        [Fact]
        public void ThrowOnError_Error_ShouldCallBeforeThrowActionAndThrowRfcException()
        {
            // Arrange
            RfcResultCode resultCode = RfcResultCode.RFC_CLOSED;
            var errorInfo = new RfcErrorInfo { Message = "Connection closed" };
            var beforeThrowActionMock = new Mock<Action>();

            // Act
            Action action = () => resultCode.ThrowOnError(errorInfo, beforeThrowActionMock.Object);

            // Assert
            action.Should().Throw<SapException>()
                .Which.Message.Should().Be("SAP RFC Error: RFC_CLOSED with message: Connection closed");
            beforeThrowActionMock.Verify(x => x(), Times.Once);
        }
    }
}
