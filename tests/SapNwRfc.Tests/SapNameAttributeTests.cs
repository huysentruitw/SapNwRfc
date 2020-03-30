using FluentAssertions;
using Xunit;

namespace SapNwRfc.Tests
{
    public sealed class SapNameAttributeTests
    {
        [Fact]
        public void Constructor_PassName_ShouldSetNameProperty()
        {
            // Act
            var attribute = new SapNameAttribute("SomeName");

            // Assert
            attribute.Name.Should().Be("SomeName");
        }
    }
}
