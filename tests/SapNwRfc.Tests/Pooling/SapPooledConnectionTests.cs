using System;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Pooling;
using Xunit;

namespace SapNwRfc.Tests.Pooling
{
    public sealed class SapPooledConnectionTests
    {
        private static readonly Fixture Fixture = new Fixture();
        private readonly Mock<ISapConnectionPool> _connectionPoolMock = new Mock<ISapConnectionPool>();
        private readonly Mock<ISapConnection> _rfcConnectionMock = new Mock<ISapConnection>();
        private readonly Mock<ISapFunction> _rfcFunctionMock = new Mock<ISapFunction>();

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
        public void Constructor_ShouldNotConnectionFromPool()
        {
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            new SapPooledConnection(_connectionPoolMock.Object);

            // Assert
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Never);
            _connectionPoolMock.Verify(x => x.ReturnConnection(It.IsAny<ISapConnection>()), Times.Never);
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never);
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
            _connectionPoolMock.Verify(x => x.ReturnConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never);
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
            _connectionPoolMock.Verify(x => x.ReturnConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.ForgetConnection(It.IsAny<ISapConnection>()), Times.Never);
        }

        [Fact]
        public void InvokeFunction_CommunicationFailureDuringConnect_ShouldThrowException()
        {
            // Arrange
            SapCommunicationFailedException exception = Fixture.Create<SapCommunicationFailedException>();
            _connectionPoolMock
                .SetupSequence(x => x.GetConnection(It.IsAny<CancellationToken>()))
                .Throws(exception);
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            Action action = () => connection.InvokeFunction("SomeFunction");

            // Assert
            action.Should().Throw<SapCommunicationFailedException>()
                .Which.Should().Be(exception);
        }

        [Fact]
        public void InvokeFunction_NoInput_NoOutput_ShouldForwardCall()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            connection.InvokeFunction("SomeFunction");

            // Assert
            _rfcConnectionMock.Verify(x => x.CreateFunction("SomeFunction"), Times.Once);
            _rfcFunctionMock.Verify(x => x.Invoke(), Times.Once);
        }

        [Fact]
        public void InvokeFunction_NoInput_NoOutput_CommunicationFailure_ShouldReconnectAndRetry()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var shouldThrow = true;
            _rfcFunctionMock.Setup(x => x.Invoke()).Callback(() =>
            {
                if (!shouldThrow) return;
                shouldThrow = false;
                throw new SapCommunicationFailedException(string.Empty);
            });

            // Act
            connection.InvokeFunction("SomeFunction");

            // Assert
            _connectionPoolMock.Verify(x => x.ForgetConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _rfcFunctionMock.Verify(x => x.Invoke(), Times.Exactly(2));
        }

        [Fact]
        public void InvokeFunction_Input_NoOutput_ShouldForwardCall()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var input = new { Name = "123" };

            // Act
            connection.InvokeFunction("SomeFunction", input);

            // Assert
            _rfcConnectionMock.Verify(x => x.CreateFunction("SomeFunction"), Times.Once);
            _rfcFunctionMock.Verify(x => x.Invoke(input), Times.Once);
        }

        [Fact]
        public void InvokeFunction_Input_NoOutput_CommunicationFailure_ShouldReconnectAndRetry()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var input = new { Name = "123" };
            var shouldThrow = true;
            _rfcFunctionMock.Setup(x => x.Invoke(It.IsAny<object>())).Callback(() =>
            {
                if (!shouldThrow) return;
                shouldThrow = false;
                throw new SapCommunicationFailedException(string.Empty);
            });

            // Act
            connection.InvokeFunction("SomeFunction", input);

            // Assert
            _connectionPoolMock.Verify(x => x.ForgetConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _rfcFunctionMock.Verify(x => x.Invoke(input), Times.Exactly(2));
        }

        [Fact]
        public void InvokeFunction_NoInput_Output_ShouldForwardCall()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);

            // Act
            connection.InvokeFunction<OutputModel>("SomeFunction");

            // Assert
            _rfcConnectionMock.Verify(x => x.CreateFunction("SomeFunction"), Times.Once);
            _rfcFunctionMock.Verify(x => x.Invoke<OutputModel>(), Times.Once);
        }

        [Fact]
        public void InvokeFunction_NoInput_Output_CommunicationFailure_ShouldReconnectAndRetry()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var shouldThrow = true;
            _rfcFunctionMock.Setup(x => x.Invoke<OutputModel>()).Callback(() =>
            {
                if (!shouldThrow) return;
                shouldThrow = false;
                throw new SapCommunicationFailedException(string.Empty);
            });

            // Act
            connection.InvokeFunction<OutputModel>("SomeFunction");

            // Assert
            _connectionPoolMock.Verify(x => x.ForgetConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _rfcFunctionMock.Verify(x => x.Invoke<OutputModel>(), Times.Exactly(2));
        }

        [Fact]
        public void InvokeFunction_Input_Output_ShouldForwardCall()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var input = new { Name = "123" };

            // Act
            connection.InvokeFunction<OutputModel>("SomeFunction", input);

            // Assert
            _rfcConnectionMock.Verify(x => x.CreateFunction("SomeFunction"), Times.Once);
            _rfcFunctionMock.Verify(x => x.Invoke<OutputModel>(input), Times.Once);
        }

        [Fact]
        public void InvokeFunction_Input_Output_CommunicationFailure_ShouldReconnectAndRetry()
        {
            // Arrange
            var connection = new SapPooledConnection(_connectionPoolMock.Object);
            var input = new { Name = "123" };
            var shouldThrow = true;
            _rfcFunctionMock.Setup(x => x.Invoke<OutputModel>(It.IsAny<object>())).Callback(() =>
            {
                if (!shouldThrow) return;
                shouldThrow = false;
                throw new SapCommunicationFailedException(string.Empty);
            });

            // Act
            connection.InvokeFunction<OutputModel>("SomeFunction", input);

            // Assert
            _connectionPoolMock.Verify(x => x.ForgetConnection(_rfcConnectionMock.Object), Times.Once);
            _connectionPoolMock.Verify(x => x.GetConnection(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _rfcFunctionMock.Verify(x => x.Invoke<OutputModel>(input), Times.Exactly(2));
        }

        private sealed class OutputModel
        {
            public string Name { get; set; }
        }
    }
}
