using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests
{
    public sealed class SapFunctionTests
    {
        private static readonly IntPtr RfcConnectionHandle = (IntPtr)12;
        private static readonly IntPtr FunctionDescriptionHandle = (IntPtr)34;
        private static readonly IntPtr FunctionHandle = (IntPtr)56;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        [Fact]
        public void CreateFromDescriptionHandle_ShouldCreateFunction()
        {
            // Act
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Assert
            function.Should().NotBeNull();
            RfcErrorInfo errorInfo;
            _interopMock.Verify(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void CreateFromDescriptionHandle_CreationFailed_ShouldThrowException()
        {
            // Arrange
            var errorInfo = new RfcErrorInfo { Code = RfcResultCode.RFC_NOT_FOUND };
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo));

            // Act
            Action action = () => SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Assert
            action.Should().Throw<SapException>()
                .WithMessage("SAP RFC Error: RFC_NOT_FOUND");
        }

        [Fact]
        public void HasParameter_ParameterExists_ShouldReturnTrue()
        {
            // Arrange
            IntPtr parameterDescHandle;
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetParameterDescByName(It.IsAny<IntPtr>(), "PAR123", out parameterDescHandle, out errorInfo))
                .Returns(RfcResultCode.RFC_OK);
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            var result = function.HasParameter("PAR123");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasParameter_ParameterDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            IntPtr parameterDescHandle;
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetParameterDescByName(It.IsAny<IntPtr>(), "PAR123", out parameterDescHandle, out errorInfo))
                .Returns(RfcResultCode.RFC_NOT_FOUND);
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            var result = function.HasParameter("PAR123");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Invoke_NoInput_NoOutput_ShouldInvokeFunction()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo)).Returns(FunctionHandle);
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            function.Invoke();

            // Assert
            _interopMock.Verify(x => x.Invoke(RfcConnectionHandle, FunctionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Invoke_WithInput_NoOutput_ShouldMapInput()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo)).Returns(FunctionHandle);
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            function.Invoke(new { Value = 123 });

            // Assert
            _interopMock.Verify(x => x.SetInt(FunctionHandle, "VALUE", 123, out errorInfo), Times.Once);
            _interopMock.Verify(x => x.Invoke(RfcConnectionHandle, FunctionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Invoke_NoInput_WithOutput_ShouldMapOutput()
        {
            // Arrange
            int value = 456;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo)).Returns(FunctionHandle);
            _interopMock.Setup(x => x.GetInt(FunctionHandle, "VALUE", out value, out errorInfo));
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            OutputModel result = function.Invoke<OutputModel>();

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(value);
            _interopMock.Verify(x => x.GetInt(FunctionHandle, "VALUE", out value, out errorInfo), Times.Once);
            _interopMock.Verify(x => x.Invoke(RfcConnectionHandle, FunctionHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_WithInput_WithOutput_ShouldMapInputAndOutput()
        {
            // Arrange
            int value = 456;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo)).Returns(FunctionHandle);
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out value, out errorInfo));
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            OutputModel result = function.Invoke<OutputModel>(new { Value = 123 });

            // Assert
            _interopMock.Verify(x => x.SetInt(FunctionHandle, "VALUE", 123, out errorInfo), Times.Once);
            _interopMock.Verify(x => x.Invoke(RfcConnectionHandle, FunctionHandle, out errorInfo), Times.Once);
            _interopMock.Verify(x => x.GetInt(FunctionHandle, "VALUE", out value, out errorInfo), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldDestroyFunction()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.CreateFunction(FunctionDescriptionHandle, out errorInfo)).Returns(FunctionHandle);
            ISapFunction function = SapFunction.CreateFromDescriptionHandle(_interopMock.Object, RfcConnectionHandle, FunctionDescriptionHandle);

            // Act
            function.Dispose();

            // Assert
            _interopMock.Verify(x => x.DestroyFunction(FunctionHandle, out errorInfo), Times.Once);
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as generic parameter")]
        private sealed class OutputModel
        {
            public int Value { get; set; }
        }
    }
}
