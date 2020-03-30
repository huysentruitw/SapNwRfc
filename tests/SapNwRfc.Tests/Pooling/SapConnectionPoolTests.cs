using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SapNwRfc.Pooling;
using Xunit;

namespace SapNwRfc.Tests.Pooling
{
    public sealed class SapConnectionPoolTests
    {
        private const string ConnectionString = "ABC";

        [Fact]
        public void GetConnection_ShouldOpenConnection()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(ConnectionString, connectionFactory: _ => connectionMock.Object);

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
                ConnectionString,
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
                ConnectionString,
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
                ConnectionString,
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
        public void ReturnConnection_ExceedPoolSize_GetConnectionShouldBlockAndReturnPreviousConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionString,
                poolSize: 1,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            ISapConnection connection1 = pool.GetConnection();

            // Act
            Task.Run(async () =>
            {
                await Task.Delay(150);
                pool.ReturnConnection(connection1);
            });
            var sw = new Stopwatch();
            sw.Start();
            ISapConnection connection2 = pool.GetConnection();
            sw.Stop();

            // Assert
            sw.ElapsedMilliseconds.Should().BeGreaterThan(100);
            connection2.Should().NotBeNull();
            connection2.Should().Be(connection1);
        }

        [Fact]
        public void ForgetConnection_ExceedPoolSize_GetConnectionShouldBlockAndReturnNewConnection()
        {
            // Arrange
            var pool = new SapConnectionPool(
                ConnectionString,
                poolSize: 1,
                connectionFactory: _ => Mock.Of<ISapConnection>());

            ISapConnection connection1 = pool.GetConnection();

            // Act
            Task.Run(async () =>
            {
                await Task.Delay(150);
                pool.ForgetConnection(connection1);
            });
            var sw = new Stopwatch();
            sw.Start();
            ISapConnection connection2 = pool.GetConnection();
            sw.Stop();

            // Assert
            sw.ElapsedMilliseconds.Should().BeGreaterThan(100);
            connection2.Should().NotBeNull();
            connection2.Should().NotBe(connection1);
        }

        [Fact]
        public void ForgetConnection_ShouldDisposeConnection()
        {
            // Arrange
            var connectionMock = new Mock<ISapConnection>();
            var pool = new SapConnectionPool(ConnectionString, connectionFactory: _ => connectionMock.Object);
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
            var pool = new SapConnectionPool(ConnectionString, connectionFactory: _ => connectionMock.Object);
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
                ConnectionString,
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
