using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using FluentAssertions.Specialized;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Pooling;
using Xunit;

namespace SapNwRfc.Tests.Pooling
{
    public sealed class SapConnectionPoolTests
    {
        private static readonly SapConnectionParameters ConnectionParameters = new SapConnectionParameters();

        [Fact]
        public void Constructor_WithoutConnectionFactory_ShouldNotThrow()
        {
            // Act
            Action action = () => new SapConnectionPool(ConnectionParameters, connectionFactory: null);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void GetConnection_ShouldOpenConnection()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(ConnectionParameters, connectionFactory: _ => connectionMock.Object);

            // Act
            ISapConnection connection = pool.GetConnection();

            // Assert
            connection.Should().Be(connectionMock.Object);
            connectionMock.Verify(x => x.Connect(), Times.Once);
        }

        [Fact]
        public void GetConnection_ShouldReturnDifferentConnectionsUpToPoolSize()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 2,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            // Act
            ISapConnection connection1 = pool.GetConnection();
            ISapConnection connection2 = pool.GetConnection();

            // Assert
            connection1.Should().NotBeNull();
            connection2.Should().NotBeNull();
            connection1.Should().NotBe(connection2);
        }

        [Fact]
        public void GetConnection_AfterReturnConnection_ShouldReturnSameConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 3,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            // Act
            ISapConnection connection1 = pool.GetConnection();
            pool.ReturnConnection(connection1);
            ISapConnection connection2 = pool.GetConnection();

            // Assert
            connection1.Should().NotBeNull();
            connection2.Should().Be(connection1);
        }

        [Fact]
        public void GetConnection_AfterForgetConnection_ShouldReturnDifferentConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 3,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            // Act
            ISapConnection connection1 = pool.GetConnection();
            pool.ForgetConnection(connection1);
            ISapConnection connection2 = pool.GetConnection();

            // Assert
            connection1.Should().NotBeNull();
            connection2.Should().NotBeNull();
            connection2.Should().NotBe(connection1);
        }

        [Fact]
        public void GetConnection_ConnectionFactoryReturnsNull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 1,
                connectionFactory: _ => null);

            // Act
            Action action = () => pool.GetConnection();

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetConnection_CalledTwice_ConnectionFactoryReturnsNullFirst_ShouldNotCausePoolStarvation()
        {
            // Arrange
            ISapConnection firstConnection = null;
            ISapConnection secondConnection = Mock.Of<ISapConnection>();
            var connectionFactoryMock = new Mock<Func<SapConnectionParameters, ISapConnection>>();
            connectionFactoryMock
                .SetupSequence(x => x(It.IsAny<SapConnectionParameters>()))
                .Returns(firstConnection)
                .Returns(secondConnection);
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 1,
                connectionFactory: connectionFactoryMock.Object);

            // Act
            try { pool.GetConnection(); } catch { }
            Action action = () => pool.GetConnection();

            // Assert
            action.ExecutionTime().Should().BeLessThan(100.Milliseconds());
        }

        [Fact]
        public void GetConnection_FailsToConnect_ShouldLetThroughException()
        {
            // Arrange
            var failingConnectionMock = new Mock<ISapConnection>();
            failingConnectionMock.Setup(x => x.Connect()).Throws(new SapCommunicationFailedException(default));
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 1,
                connectionFactory: _ => failingConnectionMock.Object);

            // Act
            Action action = () => pool.GetConnection();

            // Assert
            action.Should().Throw<SapCommunicationFailedException>();
        }

        [Fact]
        public void ReturnConnection_ExceedPoolSize_GetConnectionShouldBlockAndReturnPreviousConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 1,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            ISapConnection connection1 = pool.GetConnection();
            ISapConnection connection2 = null;

            // Act
            Task.Run(async () =>
            {
                await Task.Delay(150);
                pool.ReturnConnection(connection1);
            });
            Action action = () => connection2 = pool.GetConnection();

            // Assert
            ExecutionTime executionTime = action.ExecutionTime();
            executionTime.Should().BeLessThan(500.Milliseconds());
            executionTime.Should().BeGreaterThan(100.Milliseconds());
            connection2.Should().NotBeNull();
            connection2.Should().Be(connection1);
        }

        [Fact]
        public void ReturnConnection_ExceedPoolSize_GetConnectionShouldBlockAndReturnFirstReturnedConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 2,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            ISapConnection connection1 = pool.GetConnection();
            ISapConnection connection2 = pool.GetConnection();
            ISapConnection connection3 = null;

            // Act
            Task.Run(async () =>
            {
                await Task.Delay(150);
                pool.ReturnConnection(connection1);
                await Task.Delay(150);
                pool.ReturnConnection(connection2);
            });
            Action action = () => connection3 = pool.GetConnection();

            // Assert
            ExecutionTime executionTime = action.ExecutionTime();
            executionTime.Should().BeLessThan(275.Milliseconds());
            executionTime.Should().BeGreaterThan(100.Milliseconds());
            connection3.Should().NotBeNull();
            connection3.Should().Be(connection1);
        }

        [Fact]
        public void ForgetConnection_ExceedPoolSize_GetConnectionShouldBlockAndReturnNewConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionParameters,
                poolSize: 1,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            ISapConnection connection1 = pool.GetConnection();
            ISapConnection connection2 = null;

            // Act
            Task.Run(async () =>
            {
                await Task.Delay(150);
                pool.ForgetConnection(connection1);
            });
            Action action = () => connection2 = pool.GetConnection();

            // Assert
            ExecutionTime executionTime = action.ExecutionTime();
            executionTime.Should().BeLessThan(500.Milliseconds());
            executionTime.Should().BeGreaterThan(100.Milliseconds());
            connection2.Should().NotBeNull();
            connection2.Should().NotBe(connection1);
        }

        [Fact]
        public void ForgetConnection_ShouldDisposeConnection()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(ConnectionParameters, connectionFactory: _ => connectionMock.Object);
            ISapConnection connection = pool.GetConnection();

            // Act
            pool.ForgetConnection(connection);

            // Assert
            connectionMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldDisposeIdleConnections()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(ConnectionParameters, connectionFactory: _ => connectionMock.Object);
            ISapConnection connection1 = pool.GetConnection();
            ISapConnection connection2 = pool.GetConnection();
            pool.ReturnConnection(connection1);
            pool.ReturnConnection(connection2);

            // Act
            pool.Dispose();

            // Assert
            connectionMock.Verify(x => x.Dispose(), Times.Exactly(2));
        }

        [Fact]
        public void Wait_ShouldDisposeIdleConnections()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(
                ConnectionParameters,
                connectionIdleTimeout: TimeSpan.FromMilliseconds(150),
                idleDetectionInterval: TimeSpan.FromMilliseconds(25),
                connectionFactory: _ => connectionMock.Object);
            pool.ReturnConnection(pool.GetConnection());

            // Assert
            connectionMock.Verify(x => x.Dispose(), Times.Never);

            // Act
            Thread.Sleep(200);

            // Assert
            connectionMock.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
