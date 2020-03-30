using System;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests
{
    public sealed class SapConnectionTests
    {
        private static readonly IntPtr SapConnectionHandle = (IntPtr)12;
        private static readonly IntPtr FunctionDescriptionHandle = (IntPtr)34;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        [Fact]
        public void Connect_ConnectionSucceeds_ShouldOpenConnection()
        {
            // Arrange
            var parameters = new SapConnectionParameters { AppServerHost = "my-server.com" };
            var connection = new SapConnection(_interopMock.Object, parameters);

            // Act
            connection.Connect();

            // Assert
            RfcErrorInfo errorInfo;
            _interopMock.Verify(
                x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), 1, out errorInfo),
                Times.Once);
        }

        [Fact]
        public void Connect_ConnectionFailed_ShouldThrow()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            var errorInfo = new RfcErrorInfo { Code = RfcResultCode.RFC_TIMEOUT };
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo));

            // Act
            Action action = () => connection.Connect();

            // Assert
            action.Should().Throw<SapException>()
                .WithMessage("SAP RFC Error: RFC_TIMEOUT");
        }

        [Fact]
        public void Disconnect_NotConnected_ShouldNotDisconnect()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());

            // Act
            connection.Disconnect();

            // Assert
            RfcErrorInfo errorInfo;
            _interopMock.Verify(x => x.CloseConnection(It.IsAny<IntPtr>(), out errorInfo), Times.Never);
        }

        [Fact]
        public void Disconnect_Connected_ShouldDisconnect()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);

            connection.Connect();

            // Act
            connection.Disconnect();

            // Assert
            _interopMock.Verify(x => x.CloseConnection(SapConnectionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Disconnect_DisconnectionFailed_ShouldThrowException()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            _interopMock
                .Setup(x => x.CloseConnection(It.IsAny<IntPtr>(), out errorInfo))
                .Returns(RfcResultCode.RFC_CANCELED);

            connection.Connect();

            // Act
            Action action = () => connection.Disconnect();

            // Assert
            action.Should().Throw<SapException>()
                .WithMessage("SAP RFC Error: RFC_CANCELED");
        }

        [Fact]
        public void Dispose_ShouldDisconnect()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);

            connection.Connect();

            // Act
            connection.Dispose();

            // Assert
            _interopMock.Verify(x => x.CloseConnection(SapConnectionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Dispose_DisconnectionFailed_ShouldNotThrow()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            _interopMock
                .Setup(x => x.CloseConnection(It.IsAny<IntPtr>(), out errorInfo))
                .Returns(RfcResultCode.RFC_CANCELED);

            connection.Connect();

            // Act
            Action action = () => connection.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void IsConnected_Connected_ShouldReturnTrue()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            int isValid = 1;
            _interopMock
                .Setup(x => x.IsConnectionHandleValid(SapConnectionHandle, out isValid, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);

            connection.Connect();

            // Act
            bool isConnected = connection.IsValid;

            // Assert
            isConnected.Should().BeTrue();
        }

        [Fact]
        public void IsConnected_Disconnected_ShouldReturnFalse()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            int isValid = 1;
            _interopMock
                .Setup(x => x.IsConnectionHandleValid(SapConnectionHandle, out isValid, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);

            connection.Connect();
            connection.Disconnect();

            // Act
            bool isConnected = connection.IsValid;

            // Assert
            isConnected.Should().BeFalse();
        }

        [Fact]
        public void IsConnected_ConnectedButLibraryReturnsError_ShouldReturnFalse()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            int isValid = 1;
            _interopMock
                .Setup(x => x.IsConnectionHandleValid(SapConnectionHandle, out isValid, out errorInfo))
                .Returns(RfcResultCode.RFC_CLOSED);

            connection.Connect();

            // Act
            bool isConnected = connection.IsValid;

            // Assert
            isConnected.Should().BeFalse();
        }

        [Fact]
        public void IsConnected_ConnectedButLibrarySaysWereDisconnected_ShouldReturnFalse()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            int isValid = 0;
            _interopMock
                .Setup(x => x.IsConnectionHandleValid(SapConnectionHandle, out isValid, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);

            connection.Connect();

            // Act
            bool isConnected = connection.IsValid;

            // Assert
            isConnected.Should().BeFalse();
        }

        [Fact]
        public void CreateFunction_ShouldCreateFunction()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            _interopMock
                .Setup(x => x.GetFunctionDesc(SapConnectionHandle, "FunctionA", out errorInfo))
                .Returns(FunctionDescriptionHandle);

            connection.Connect();

            // Act
            ISapFunction function = connection.CreateFunction("FunctionA");

            // Assert
            function.Should().NotBeNull();
            _interopMock.Verify(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Ping_NotConnected_ShouldReturnFalse()
        {
            // Arrange
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());

            // Act
            var pingResult = connection.Ping();

            // Assert
            pingResult.Should().BeFalse();
        }

        [Fact]
        public void Ping_Connected_SuccessfulPing_ShouldReturnTrue()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            _interopMock
                .Setup(x => x.Ping(SapConnectionHandle, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            connection.Connect();

            // Act
            var pingResult = connection.Ping();

            // Assert
            pingResult.Should().BeTrue();
        }

        [Fact]
        public void Ping_Connected_PingTimeout_ShouldReturnFalse()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.OpenConnection(It.IsAny<RfcConnectionParameter[]>(), It.IsAny<uint>(), out errorInfo))
                .Returns(SapConnectionHandle);
            _interopMock
                .Setup(x => x.Ping(SapConnectionHandle, out errorInfo))
                .Returns(RfcResultCode.RFC_TIMEOUT);
            var connection = new SapConnection(_interopMock.Object, new SapConnectionParameters());
            connection.Connect();

            // Act
            var pingResult = connection.Ping();

            // Assert
            pingResult.Should().BeFalse();
        }
    }
}
