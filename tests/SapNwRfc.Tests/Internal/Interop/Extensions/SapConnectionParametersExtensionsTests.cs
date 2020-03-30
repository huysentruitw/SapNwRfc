using System.Linq;
using FluentAssertions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal.Interop.Extensions
{
    public sealed class SapConnectionParametersExtensionsTests
    {
        [Fact]
        public void ToInterop_NoValueSet_ShouldReturnEmptyArray()
        {
            // Arrange
            var parameters = new SapConnectionParameters();

            // Act
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            // Assert
            interopParameters.Should().BeEmpty();
        }

        [Fact]
        public void ToInterop_ShouldMapNonNullValues()
        {
            // Arrange
            var parameters = new SapConnectionParameters
            {
                Name = "SomeName",
                Language = "EN",
            };

            // Act
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            // Assert
            interopParameters.Should().HaveCount(2);
            interopParameters.First().Should().BeEquivalentTo(new { Name = "NAME", Value = "SomeName" });
            interopParameters.Last().Should().BeEquivalentTo(new { Name = "LANG", Value = "EN" });
        }

        [Fact]
        public void ToInterop_ShouldUseNameFromAttribute()
        {
            // Arrange
            var parameters = new SapConnectionParameters
            {
                RepositoryPassword = "SomeRepoPassword",
            };

            // Act
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            // Assert
            interopParameters.First().Name.Should().Be("REPOSITORY_PASSWD");
        }
    }
}
