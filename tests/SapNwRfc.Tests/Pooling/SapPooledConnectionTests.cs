using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Pooling;
using Xunit;

namespace SapNwRfc.Tests.Pooling
{
    public sealed class SapPooledConnectionTests
    {
        private readonly Mock<ISapConnectionPool> _connectionPoolMock = new Mock<ISapConnectionPool>();
        private readonly Mock<ISapConnection> _rfcConnectionMock = new Mock<ISapConnection>();
        private readonly Mock<ISapFunction> _rfcFunctionMock = new Mock<ISapFunction>();

        private static readonly object InputModel = new { Name = "123" };

        private static readonly Dictionary<InvokeFlavor, Action<SapPooledConnection>> InvokeActions = new Dictionary<InvokeFlavor, Action<SapPooledConnection>>
        {
            { InvokeFlavor.NoInputNoOutput, connection => connection.InvokeFunction("SomeFunction") },
            { InvokeFlavor.InputOnly, connection => connection.InvokeFunction("SomeFunction", InputModel) },
            { InvokeFlavor.OutputOnly, connection => connection.InvokeFunction<OutputModel>("SomeFunction") },
            { InvokeFlavor.InputOutput, connection => connection.InvokeFunction<OutputModel>("SomeFunction", InputModel) },
        };

        private static readonly Dictionary<InvokeFlavor, Action<Mock<ISapFunction>, Times>> VerifyActions = new Dictionary<InvokeFlavor, Action<Mock<ISapFunction>, Times>>
        {
            { InvokeFlavor.NoInputNoOutput, (mock, times) => mock.Verify(x => x.Invoke(), times) },
            { InvokeFlavor.InputOnly, (mock, times) => mock.Verify(x => x.Invoke(InputModel), times) },
            { InvokeFlavor.OutputOnly, (mock, times) => mock.Verify(x => x.Invoke<OutputModel>(), times) },
            { InvokeFlavor.InputOutput, (mock, times) => mock.Verify(x => x.Invoke<OutputModel>(InputModel), times) },
        };

        public enum InvokeFlavor
        {
            NoInputNoOutput,
            InputOnly,
            OutputOnly,
            InputOutput,
        }

        public SapPooledConnectionTests()
        {
            _connectionPoolMock
                .Setup(x => x.GetConnection(It.IsAny<CancellationToken>()))
                .Returns(_rfcConnectionMock.Object);

            _rfcConnectionMock
                .Setup(x => x.CreateFunction(It.IsAny<string>()))
                .Returns(_rfcFunctionMock.Object);
        }

        [Fact]
        public void Constructor_ShouldNotGetConnectionFromPool()
        {
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            new SapPooledConnection(_connectionPoolMock.Object);

            // Assert
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Never());
            _connectionPoolMock.Verify(x => x.ReturnConnection(It.IsAny<ISapConnection>()), Times.Never());
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never());
        }

        [Fact]
        public void Dispose_RightAfterConstruction_ShouldNotReturnConnectionToPool()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            connection.Dispose();

            // Assert
            _connectionPoolMock.Verify(x => x.ReturnConnection(It.IsAny<ISapConnection>()), Times.Never());
        }

        [Fact]
        public void Dispose_ShouldReturnConnectionToPool()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            connection.InvokeFunction("Test");

            // Act
            connection.Dispose();

            // Assert
            _connectionPoolMock.Verify(x => x.ReturnConnection(_rfcConnectionMock.Object), Times.Once());
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never());
        }

        [Fact]
        public void Dispose_Twice_ShouldOnlyReturnConnectionToPoolOnce()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            connection.InvokeFunction("Test");

            // Act
            connection.Dispose();
            connection.Dispose();

            // Assert
            _connectionPoolMock.Verify(x => x.ReturnConnection(_rfcConnectionMock.Object), Times.Once());
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never());
        }

        [Theory]
        [InlineData(InvokeFlavor.NoInputNoOutput)]
        [InlineData(InvokeFlavor.InputOnly)]
        [InlineData(InvokeFlavor.OutputOnly)]
        [InlineData(InvokeFlavor.InputOutput)]
        public void InvokeFunction_ShouldForwardCall(InvokeFlavor invokeFlavor)
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            InvokeActions[invokeFlavor](connection);

            // Assert
            _rfcConnectionMock.Verify(x => x.CreateFunction("SomeFunction"), Times.Once());
            VerifyActions[invokeFlavor](_rfcFunctionMock, Times.Once());
        }

        [Theory]
        [InlineData(InvokeFlavor.NoInputNoOutput)]
        [InlineData(InvokeFlavor.InputOnly)]
        [InlineData(InvokeFlavor.OutputOnly)]
        [InlineData(InvokeFlavor.InputOutput)]
        public void InvokeFunction_CalledTwice_ShouldReuseUnderlyingConnection(InvokeFlavor invokeFlavor)
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            InvokeActions[invokeFlavor](connection);
            InvokeActions[invokeFlavor](connection);

            // Assert
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [InlineData(InvokeFlavor.NoInputNoOutput)]
        [InlineData(InvokeFlavor.InputOnly)]
        [InlineData(InvokeFlavor.OutputOnly)]
        [InlineData(InvokeFlavor.InputOutput)]
        public void InvokeFunction_CommunicationFailureDuringFirstInvoke_ShouldReconnectAndRetry(InvokeFlavor invokeFlavor)
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            _rfcFunctionMock.SetupSequence(x => x.Invoke()).Throws(new SapCommunicationFailedException(default));
            _rfcFunctionMock.SetupSequence(x => x.Invoke(InputModel)).Throws(new SapCommunicationFailedException(default));
            _rfcFunctionMock.SetupSequence(x => x.Invoke<OutputModel>()).Throws(new SapCommunicationFailedException(default));
            _rfcFunctionMock.SetupSequence(x => x.Invoke<OutputModel>(InputModel)).Throws(new SapCommunicationFailedException(default));

            // Act
            InvokeActions[invokeFlavor](connection);

            // Assert
            _connectionPoolMock.Verify(x => x.ForgetConnection(_rfcConnectionMock.Object), Times.Once());
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Exactly(2));
            VerifyActions[invokeFlavor](_rfcFunctionMock, Times.Exactly(2));
        }

        [Theory]
        [InlineData(InvokeFlavor.NoInputNoOutput)]
        [InlineData(InvokeFlavor.InputOnly)]
        [InlineData(InvokeFlavor.OutputOnly)]
        [InlineData(InvokeFlavor.InputOutput)]
        public void InvokeFunction_CommunicationFailureDuringGetConnection_ShouldThrowSapCommunicationFailedException_ShouldNotCallForgetConnection(InvokeFlavor invokeFlavor)
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            _connectionPoolMock
                .SetupSequence(x => x.GetConnection(It.IsAny<CancellationToken>()))
                .Throws(new SapCommunicationFailedException(default))
                .Returns(_rfcConnectionMock.Object);

            // Act
            Action action = () => InvokeActions[invokeFlavor](connection);

            // Assert
            action.Should().Throw<SapCommunicationFailedException>();
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Once());
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never());
            VerifyActions[invokeFlavor](_rfcFunctionMock, Times.Never());
        }

        private sealed class OutputModel
        {
            public string Name { get; set; }
        }
    }
}
