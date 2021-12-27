using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests
{
    public sealed class SapServerConnectionTests
    {
        private static readonly Fixture Fixture = new Fixture();
        private static readonly IntPtr RfcConnectionHandle = (IntPtr)12;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        public SapServerConnectionTests()
        {
            new SupportMutableValueTypesCustomization().Customize(Fixture);
        }

        [Fact]
        public void GetAttributes_InteropSucceeds_ShouldReturnAttributesFromInterop()
        {
            // Arrange
            RfcAttributes rfcAttributes = Fixture.Create<RfcAttributes>();
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetConnectionAttributes(RfcConnectionHandle, out rfcAttributes, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);
            var serverConnection = new SapServerConnection(_interopMock.Object, RfcConnectionHandle);

            // Act
            SapAttributes sapAttributes = serverConnection.GetAttributes();

            // Assert
            sapAttributes.Should().BeEquivalentTo(rfcAttributes, ctx => ctx
                .ComparingByMembers<RfcAttributes>()
                .ComparingByMembers<SapAttributes>()
                .ExcludingMissingMembers());
        }

        [Fact]
        public void GetAttributes_InteropFails_ShouldThrowSapException()
        {
            // Arrange
            RfcAttributes rfcAttributes = Fixture.Create<RfcAttributes>();
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetConnectionAttributes(RfcConnectionHandle, out rfcAttributes, out errorInfo))
                .Returns(RfcResultCode.RFC_NOT_FOUND);
            var serverConnection = new SapServerConnection(_interopMock.Object, RfcConnectionHandle);

            // Act
            Action action = () => serverConnection.GetAttributes();

            // Assert
            action.Should().Throw<SapException>();
        }
    }
}
