using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal
{
public sealed class InputMapperTests
    {
        private static readonly Fixture Fixture = new Fixture();
        private static readonly IntPtr DataHandle = (IntPtr)123;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        [Fact]
        public void Apply_NullInput_ShouldNotThrow()
        {
            // Act
            Action action = () => InputMapper.Apply(_interopMock.Object, DataHandle, null);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Apply_String_ShouldMapAsString()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeString = "Hello" });

            // Assert
            _interopMock.Verify(x => x.SetString(DataHandle, "SOMESTRING", "Hello", 5, out errorInfo));
        }

        [Fact]
        public void Apply_NullString_ShouldNotMapString()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeString = (string)null });

            // Assert
            _interopMock.Verify(
                x => x.SetString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<uint>(), out errorInfo),
                Times.Never);
        }

        [Fact]
        public void Apply_Int_ShouldMapAsInt()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeInt = 555 });

            // Assert
            _interopMock.Verify(x => x.SetInt(DataHandle, "SOMEINT", 555, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_Long_ShouldMapAsInt8()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeLong = 123L });

            // Assert
            _interopMock.Verify(x => x.SetInt8(DataHandle, "SOMELONG", 123L, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_Double_ShouldMapAsFloat()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeDouble = 1234.5d });

            // Assert
            _interopMock.Verify(x => x.SetFloat(DataHandle, "SOMEDOUBLE", 1234.5d, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_Decimal_ShouldMapAsFormattedString()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeDecimal = 123.4M });

            // Assert
            _interopMock.Verify(x => x.SetString(DataHandle, "SOMEDECIMAL", "123.4", 5, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_DateTime_ShouldMapAsDate()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            var date = new DateTime(2020, 4, 1);

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeDate = date });

            // Assert
            _interopMock.Verify(x => x.SetDate(
                DataHandle,
                "SOMEDATE",
                It.Is<char[]>(y => y.SequenceEqual("20200401")),
                out errorInfo));
        }

        [Fact]
        public void Apply_NullableDateTime_HasValue_ShouldMapAsDate()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            DateTime? date = new DateTime(2020, 4, 1);

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeDate = date });

            // Assert
            _interopMock.Verify(x => x.SetDate(
                DataHandle,
                "SOMEDATE",
                It.Is<char[]>(y => y.SequenceEqual("20200401")),
                out errorInfo));
        }

        [Fact]
        public void Apply_NullableDateTime_NullValue_ShouldMapAsZeroDate()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeDate = (DateTime?)null });

            // Assert
            _interopMock.Verify(
                x => x.SetDate(
                    DataHandle,
                    "SOMEDATE",
                    It.Is<char[]>(y => y.SequenceEqual("00000000")),
                    out errorInfo),
                Times.Once);
        }

        [Fact]
        public void Apply_TimeSpan_ShouldMapAsTime()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            var time = new TimeSpan(23, 45, 16);

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeTime = time });

            // Assert
            _interopMock.Verify(
                x => x.SetTime(
                    DataHandle,
                    "SOMETIME",
                    It.Is<char[]>(y => y.SequenceEqual("234516")),
                    out errorInfo),
                Times.Once);
        }

        [Fact]
        public void Apply_NullableTimeSpan_HasValue_ShouldMapAsTime()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            TimeSpan? time = new TimeSpan(23, 45, 16);

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeTime = time });

            // Assert
            _interopMock.Verify(
                x => x.SetTime(
                    DataHandle,
                    "SOMETIME",
                    It.Is<char[]>(y => y.SequenceEqual("234516")),
                    out errorInfo),
                Times.Once);
        }

        [Fact]
        public void Apply_NullableTimeSpan_NullValue_ShouldMapAsZeroTime()
        {
            // Arrange
            RfcErrorInfo errorInfo;

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, new { SomeTime = (TimeSpan?)null });

            // Assert
            _interopMock.Verify(
                x => x.SetTime(
                    DataHandle,
                    "SOMETIME",
                    It.Is<char[]>(y => y.SequenceEqual("000000")),
                    out errorInfo),
                Times.Once);
        }

        [Fact]
        public void Apply_Array_ShouldMapAsTable()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            var model = new { SomeArray = Fixture.CreateMany<ArrayElement>(2).ToArray() };

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, model);

            // Assert
            IntPtr tableHandle;
            _interopMock.Verify(x => x.GetTable(DataHandle, "SOMEARRAY", out tableHandle, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_Array_ShouldMapRowsAndValues()
        {
            // Arrange
            const int numberOfRows = 5;
            var tableHandle = (IntPtr)1235;
            var lineHandle = (IntPtr)2245;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetTable(DataHandle, "SOMEARRAY", out tableHandle, out errorInfo));
            _interopMock.Setup(x => x.AppendNewRow(It.IsAny<IntPtr>(), out errorInfo)).Returns(lineHandle);
            var model = new { SomeArray = Fixture.CreateMany<ArrayElement>(numberOfRows).ToArray() };

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, model);

            // Assert
            _interopMock.Verify(x => x.AppendNewRow(tableHandle, out errorInfo), Times.Exactly(numberOfRows));
            foreach (ArrayElement element in model.SomeArray)
            {
                var length = (uint)element.Value.Length;
                _interopMock.Verify(
                    x => x.SetString(lineHandle, "VALUE", element.Value, length, out errorInfo),
                    Times.Once);
            }
        }

        [Fact]
        public void Apply_Structure_ShouldMapAsStructure()
        {
            // Arrange
            var structHandle = (IntPtr)44553;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetStructure(It.IsAny<IntPtr>(), It.IsAny<string>(), out structHandle, out errorInfo));
            var model = new StructureModel { Structure = new Structure { Value = 224 } };

            // Act
            InputMapper.Apply(_interopMock.Object, DataHandle, model);

            // Assert
            _interopMock.Verify(x => x.GetStructure(DataHandle, "STRUCTURE", out structHandle, out errorInfo), Times.Once);
            _interopMock.Verify(x => x.SetInt(structHandle, "VALUE", 224, out errorInfo), Times.Once);
        }

        [Fact]
        public void Apply_UnknownTypeThatCannotBeConstructed_ShouldThrowException()
        {
            // Arrange & Act
            Action action = () => InputMapper.Apply(_interopMock.Object, DataHandle, new { UnknownType = 1.0f });

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("No matching field constructor found");
        }

        private sealed class ArrayElement
        {
            public string Value { get; set; } = "123";
        }

        private sealed class StructureModel
        {
            public Structure Structure { get; set; }
        }

        private sealed class Structure
        {
            public int Value { get; set; }
        }
    }
}
