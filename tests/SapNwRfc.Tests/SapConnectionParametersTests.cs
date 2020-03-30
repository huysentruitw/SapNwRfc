using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace SapNwRfc.Tests
{
    public sealed class SapConnectionParametersTests
    {
        private static readonly Fixture Fixture = new Fixture();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Parse_InvalidConnectionString_ShouldThrowArgumentException(string connectionString)
        {
            // Act
            Action action = () => SapConnectionParameters.Parse(connectionString);

            // Assert
            action.Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("connectionString");
        }

        [Fact]
        public void Parse_ShouldSetProperties()
        {
            // Arrange
            const string connectionString = "AppServerHost=MyFancyHost;User= SomeUsername; Password = SomePassword ";

            // Act
            var parameters = SapConnectionParameters.Parse(connectionString);

            // Assert
            parameters.Should().NotBeNull();
            parameters.AppServerHost.Should().Be("MyFancyHost");
            parameters.User.Should().Be("SomeUsername");
            parameters.Password.Should().Be("SomePassword");
        }

        [Fact]
        public void Parse_AllProperties()
        {
            // Arrange
            SapConnectionParameters expectedParameters = Fixture.Create<SapConnectionParameters>();
            string connectionString = typeof(SapConnectionParameters)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Aggregate(new StringBuilder(), (sb, propertyInfo) =>
                {
                    object value = propertyInfo.GetValue(expectedParameters);
                    sb.Append($"{propertyInfo.Name}={value};");
                    return sb;
                })
                .ToString();

            // Act
            var parameters = SapConnectionParameters.Parse(connectionString);

            // Assert
            parameters.Should().BeEquivalentTo(expectedParameters);
        }
    }
}
