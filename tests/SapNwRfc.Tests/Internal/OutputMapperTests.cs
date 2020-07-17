using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as generic output types")]
    public sealed class OutputMapperTests
    {
        private static readonly IntPtr DataHandle = (IntPtr)123;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        private delegate void GetStringCallback(IntPtr dataHandle, string name, char[] buffer, uint bufferLength, out uint stringLength, out RfcErrorInfo errorInfo);

        [Theory]
        [InlineData("")]
        [InlineData("hello")]
        public void Extract_String_ShouldMapFromString(string value)
        {
            // Assert
            string stringValue = value;
            uint stringLength = (uint)stringValue.Length;
            RfcErrorInfo errorInfo;
            var resultCodeQueue = new Queue<RfcResultCode>();
            resultCodeQueue.Enqueue(RfcResultCode.RFC_BUFFER_TOO_SMALL);
            resultCodeQueue.Enqueue(RfcResultCode.RFC_OK);
            _interopMock
                .Setup(x => x.GetString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), It.IsAny<uint>(), out stringLength, out errorInfo))
                .Callback(new GetStringCallback((IntPtr dataHandle, string name, char[] buffer, uint bufferLength, out uint sl, out RfcErrorInfo ei) =>
                {
                    ei = default;
                    sl = stringLength;
                    if (buffer.Length <= 0 || bufferLength <= 0) return;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(resultCodeQueue.Dequeue);

            // Act
            StringModel result = OutputMapper.Extract<StringModel>(_interopMock.Object, DataHandle);

            // Assert
            uint discard;
            _interopMock.Verify(
                x => x.GetString(DataHandle, "STRINGVALUE", Array.Empty<char>(), 0, out discard, out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetString(DataHandle, "STRINGVALUE", It.IsAny<char[]>(), stringLength + 1, out discard, out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.StringValue.Should().Be(stringValue);
        }

        [Fact]
        public void Extract_EmptyString_ShouldMapAsEmptyString()
        {
            // Arrange
            RfcErrorInfo errorInfo;
            uint stringLength = 0;
            _interopMock.Setup(x => x.GetString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), It.IsAny<uint>(), out stringLength, out errorInfo));

            // Act
            StringModel result = OutputMapper.Extract<StringModel>(_interopMock.Object, DataHandle);

            // Assert
            uint discard;
            _interopMock.Verify(
                x => x.GetString(DataHandle, "STRINGVALUE", Array.Empty<char>(), 0, out discard, out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.StringValue.Should().BeEmpty();
        }

        private sealed class StringModel
        {
            public string StringValue { get; set; }
        }

        [Theory]
        [InlineData(123)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void Extract_Int_ShouldMapFromInt(int value)
        {
            // Arrange
            int intValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetInt(DataHandle, "INTVALUE", out intValue, out errorInfo));

            // Act
            IntModel result = OutputMapper.Extract<IntModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.IntValue.Should().Be(intValue);
        }

        private sealed class IntModel
        {
            public int IntValue { get; set; }
        }

        [Theory]
        [InlineData(66778L)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public void Extract_Long_ShouldMapFromInt8(long value)
        {
            // Arrange
            var longValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetInt8(DataHandle, "LONGVALUE", out longValue, out errorInfo));

            // Act
            LongModel result = OutputMapper.Extract<LongModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.LongValue.Should().Be(longValue);
        }

        private sealed class LongModel
        {
            public long LongValue { get; set; }
        }

        [Theory]
        [InlineData(1234.5d)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        public void Extract_Double_ShouldMapFromFloat(double value)
        {
            // Arrange
            var doubleValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetFloat(DataHandle, "DOUBLEVALUE", out doubleValue, out errorInfo));

            // Act
            DoubleModel result = OutputMapper.Extract<DoubleModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.DoubleValue.Should().Be(doubleValue);
        }

        private sealed class DoubleModel
        {
            public double DoubleValue { get; set; }
        }

        [Theory]
        [InlineData(123.56)]
        [InlineData(-123.56)]
        public void Extract_Decimal_ShouldMapFromDecimalString(decimal value)
        {
            // Assert
            string stringValue = value.ToString("G", CultureInfo.InvariantCulture);
            uint stringLength = (uint)stringValue.Length;
            RfcErrorInfo errorInfo;
            var resultCodeQueue = new Queue<RfcResultCode>();
            resultCodeQueue.Enqueue(RfcResultCode.RFC_BUFFER_TOO_SMALL);
            resultCodeQueue.Enqueue(RfcResultCode.RFC_OK);
            _interopMock
                .Setup(x => x.GetString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), It.IsAny<uint>(), out stringLength, out errorInfo))
                .Callback(new GetStringCallback((IntPtr dataHandle, string name, char[] buffer, uint bufferLength, out uint sl, out RfcErrorInfo ei) =>
                {
                    ei = default;
                    sl = stringLength;
                    if (buffer.Length <= 0 || bufferLength <= 0) return;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(resultCodeQueue.Dequeue);

            // Act
            DecimalModel result = OutputMapper.Extract<DecimalModel>(_interopMock.Object, DataHandle);

            // Assert
            uint discard;
            _interopMock.Verify(
                x => x.GetString(DataHandle, "DECIMALVALUE", Array.Empty<char>(), 0, out discard, out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetString(DataHandle, "DECIMALVALUE", It.IsAny<char[]>(), stringLength + 1, out discard, out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.DecimalValue.Should().Be(value);
        }

        [Fact]
        public void Extract_Decimal_EmptyString_ShouldMapToDecimalZero()
        {
            // Assert
            uint stringLength = 0;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), It.IsAny<uint>(), out stringLength, out errorInfo));

            // Act
            DecimalModel result = OutputMapper.Extract<DecimalModel>(_interopMock.Object, DataHandle);

            // Assert
            uint discard;
            _interopMock.Verify(
                x => x.GetString(DataHandle, "DECIMALVALUE", Array.Empty<char>(), 0, out discard, out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.DecimalValue.Should().Be(0M);
        }

        private sealed class DecimalModel
        {
            public decimal DecimalValue { get; set; }
        }

        private delegate void GetDateCallback(IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo errorInfo);

        [Fact]
        public void Extract_DateTime_ShouldMapFromDate()
        {
            // Arrange
            const string value = "20200405";
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetDate(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetDateCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            DateTimeModel result = OutputMapper.Extract<DateTimeModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetDate(DataHandle, "DATETIMEVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetDate(DataHandle, "NULLABLEDATETIMEVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.DateTimeValue.Should().Be(new DateTime(2020, 04, 05));
            result.NullableDateTimeValue.Should().Be(new DateTime(2020, 04, 05));
        }

        [Theory]
        [InlineData("00000000")]
        [InlineData("        ")]
        [InlineData("abcdefgh")]
        public void Extract_NonNullableDateTime_ZeroOrEmptyOrInvalidDate_ShouldMapToMinimumDateTime(string value)
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetDate(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetDateCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            DateTimeModel result = OutputMapper.Extract<DateTimeModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetDate(DataHandle, "DATETIMEVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.DateTimeValue.Should().Be(DateTime.MinValue);
        }

        [Theory]
        [InlineData("00000000")]
        [InlineData("        ")]
        [InlineData("abcdefgh")]
        public void Extract_NullableDateTime_ZeroOrEmptyOrInvalidDate_ShouldMapToNullDateTime(string value)
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetDate(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetDateCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            DateTimeModel result = OutputMapper.Extract<DateTimeModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetDate(DataHandle, "NULLABLEDATETIMEVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.NullableDateTimeValue.Should().BeNull();
        }

        private sealed class DateTimeModel
        {
            public DateTime DateTimeValue { get; set; }

            public DateTime? NullableDateTimeValue { get; set; }
        }

        private delegate void GetTimeCallback(IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo errorInfo);

        [Fact]
        public void Extract_TimeSpan_ShouldMapFromTime()
        {
            // Arrange
            const string value = "123456";
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetTime(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetTimeCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            TimeSpanModel result = OutputMapper.Extract<TimeSpanModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetTime(DataHandle, "TIMESPANVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetTime(DataHandle, "NULLABLETIMESPANVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.TimeSpanValue.Should().Be(new TimeSpan(12, 34, 56));
            result.NullableTimeSpanValue.Should().Be(new TimeSpan(12, 34, 56));
        }

        [Theory]
        [InlineData("000000")]
        [InlineData("      ")]
        [InlineData("abcdef")]
        public void Extract_NonNullableTimeSpan_ZeroOrEmptyOrInvalidTime_ShouldMapToZeroTimeSpan(string value)
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetTime(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetTimeCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            TimeSpanModel result = OutputMapper.Extract<TimeSpanModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetTime(DataHandle, "TIMESPANVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.TimeSpanValue.Should().Be(TimeSpan.Zero);
        }

        [Theory]
        [InlineData("000000")]
        [InlineData("      ")]
        [InlineData("abcdef")]
        public void Extract_NullableTimeSpan_ZeroOrEmptyOrInvalidTime_ShouldMapToNullTimeSpan(string value)
        {
            // Arrange
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetTime(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetTimeCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    Array.Copy(value.ToCharArray(), buffer, value.Length);
                    ei = default;
                }));

            // Act
            TimeSpanModel result = OutputMapper.Extract<TimeSpanModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetTime(DataHandle, "NULLABLETIMESPANVALUE", It.IsAny<char[]>(), out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.NullableTimeSpanValue.Should().BeNull();
        }

        private sealed class TimeSpanModel
        {
            public TimeSpan TimeSpanValue { get; set; }

            public TimeSpan? NullableTimeSpanValue { get; set; }
        }

        [Fact]
        public void Extract_TableWithRows_ShouldMapToArrayOfElements()
        {
            // Arrange
            var tableHandle = (IntPtr)3334;
            var rowHandle = (IntPtr)4445;
            uint rowCount = 3;
            int intValue = 888;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetTable(It.IsAny<IntPtr>(), It.IsAny<string>(), out tableHandle, out errorInfo));
            _interopMock.Setup(x => x.GetRowCount(It.IsAny<IntPtr>(), out rowCount, out errorInfo));
            _interopMock.Setup(x => x.GetCurrentRow(It.IsAny<IntPtr>(), out errorInfo)).Returns(rowHandle);
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            // Act
            ArrayModel result = OutputMapper.Extract<ArrayModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetTable(DataHandle, "ELEMENTS", out tableHandle, out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetRowCount(tableHandle, out rowCount, out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetCurrentRow(tableHandle, out errorInfo),
                Times.Exactly(3));
            _interopMock.Verify(
                x => x.GetInt(rowHandle, "VALUE", out intValue, out errorInfo),
                Times.Exactly(3));
            _interopMock.Verify(x => x.MoveToNextRow(tableHandle, out errorInfo), Times.Exactly(3));
            result.Should().NotBeNull();
            result.Elements.Should().HaveCount(3);
            result.Elements.First().Value.Should().Be(888);
        }

        [Fact]
        public void Extract_TableWithLessRowsThanAnnounced_ShouldReturnExtractedRows()
        {
            // Arrange
            var tableHandle = (IntPtr)3334;
            var rowHandle = (IntPtr)4445;
            uint rowCount = 3;
            int intValue = 888;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetTable(It.IsAny<IntPtr>(), It.IsAny<string>(), out tableHandle, out errorInfo));
            _interopMock.Setup(x => x.GetRowCount(It.IsAny<IntPtr>(), out rowCount, out errorInfo));
            _interopMock.Setup(x => x.GetCurrentRow(It.IsAny<IntPtr>(), out errorInfo)).Returns(rowHandle);
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            _interopMock
                .Setup(x => x.MoveToNextRow(It.IsAny<IntPtr>(), out errorInfo))
                .Returns(RfcResultCode.RFC_TABLE_MOVE_EOF);

            // Act
            ArrayModel result = OutputMapper.Extract<ArrayModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.Elements.Should().HaveCount(1);
        }

        private sealed class ArrayModel
        {
            public ArrayElement[] Elements { get; set; }
        }

        private sealed class ArrayElement
        {
            public int Value { get; set; }
        }

        [Fact]
        public void Extract_Structure_ShouldMapToNestedObject()
        {
            // Arrange
            var structHandle = (IntPtr)443534;
            var intValue = 123;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetStructure(It.IsAny<IntPtr>(), It.IsAny<string>(), out structHandle, out errorInfo));
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            // Act
            NestedModel result = OutputMapper.Extract<NestedModel>(_interopMock.Object, DataHandle);

            // Assert
            _interopMock.Verify(
                x => x.GetStructure(DataHandle, "INNERMODEL", out structHandle, out errorInfo),
                Times.Once);
            _interopMock.Verify(
                x => x.GetInt(structHandle, "VALUE", out intValue, out errorInfo),
                Times.Once);
            result.Should().NotBeNull();
            result.InnerModel.Should().NotBeNull();
            result.InnerModel.Value.Should().Be(123);
        }

        private sealed class NestedModel
        {
            public InnerModel InnerModel { get; set; }
        }

        private sealed class InnerModel
        {
            public int Value { get; set; }
        }

        [Fact]
        public void Extract_PropertyWithSapNameAttribute_ShouldMapUsingRfcName()
        {
            // Arrange
            int value = 334;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetInt(DataHandle, "I34", out value, out errorInfo));

            // Act
            IntAttributeModel result = OutputMapper.Extract<IntAttributeModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.IntValue.Should().Be(334);
        }

        private sealed class IntAttributeModel
        {
            [SapName("I34")]
            public int IntValue { get; set; }
        }

        [Fact]
        public void Extract_ClassWithOnInitializeFunction_ShouldCallOnInitialize()
        {
            // Arrange

            // Act
            OnIntializeAttributeModel result = OutputMapper.Extract<OnIntializeAttributeModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.OnInitializeCalled.Should().Be(1);
        }

        private sealed class OnIntializeAttributeModel
        {
            public int OnInitializeCalled { get; set; } = 0;

            public void OnInitialize()
            {
                OnInitializeCalled = 1;
            }
        }

        [Fact]
        public void Extract_ClassWithNestedOnInitializeFunction_ShouldCallAllOnInitialize()
        {
            // Arrange

            // Act
            NestedOnIntializeAttributeModel result = OutputMapper.Extract<NestedOnIntializeAttributeModel>(_interopMock.Object, DataHandle);

            // Assert
            result.Should().NotBeNull();
            result.OnInitializeCalled.Should().Be(1);
            result.NestedObject.OnInitializeCalled.Should().Be(1);
        }

        private sealed class NestedOnIntializeAttributeModel
        {
            public OnIntializeAttributeModel NestedObject { get; set; }

            public int OnInitializeCalled { get; set; } = 0;

            public void OnInitialize()
            {
                OnInitializeCalled = 1;
            }
        }

        [Fact]
        public void Extract_UnknownTypeThatCannotBeExtracted_ShouldThrowException()
        {
            // Arrange & Act
            Action action = () => OutputMapper.Extract<UnknownTypeModel>(_interopMock.Object, DataHandle);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("No matching extract method found for type Single");
        }

        private sealed class UnknownTypeModel
        {
            public float Float { get; set; }
        }
    }
}
