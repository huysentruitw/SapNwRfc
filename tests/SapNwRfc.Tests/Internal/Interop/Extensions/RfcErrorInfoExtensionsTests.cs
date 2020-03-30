using System;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal.Interop.Extensions
{
    public sealed class RfcErrorInfoExtensionsTests
    {
        [Fact]
        public void ThrowOnError_NoError_ShouldNotThrow()
        {
            // Arrange
            var errorInfo = new RfcErrorInfo { Code = RfcResultCode.RFC_OK };

            // Act
            Action action = () => errorInfo.ThrowOnError();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowOnError_NoError_ShouldNotCallBeforeThrowAction()
        {
            // Arrange
            var errorInfo = new RfcErrorInfo { Code = RfcResultCode.RFC_OK };
            var beforeThrowActionMock = new Mock<Action>();

            // Act
            errorInfo.ThrowOnError(beforeThrowActionMock.Object);

            // Assert
            beforeThrowActionMock.Verify(x => x(), Times.Never);
        }

        [Fact]
        public void ThrowOnError_Error_ShouldCallBeforeThrowActionAndThrowRfcException()
        {
            // Arrange
            var errorInfo = new RfcErrorInfo
            {
                Code = RfcResultCode.RFC_CLOSED,
                Message = "Connection closed",
            };
            var beforeThrowActionMock = new Mock<Action>();

            // Act
            Action action = () => errorInfo.ThrowOnError(beforeThrowActionMock.Object);

            // Assert
            action.Should().Throw<SapException>()
                .Which.Message.Should().Be("SAP RFC Error: RFC_CLOSED with message: Connection closed");
            beforeThrowActionMock.Verify(x => x(), Times.Once);
        }
    }
}
