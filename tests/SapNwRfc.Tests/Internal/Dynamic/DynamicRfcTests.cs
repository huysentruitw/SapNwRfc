using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Moq;
using SapNwRfc.Internal.Dynamic;
using SapNwRfc.Internal.Interop;
using Xunit;

namespace SapNwRfc.Tests.Internal.Dynamic
{
    public sealed class DynamicRfcTests
    {
        private static readonly IntPtr DataHandle = (IntPtr)123;
        private readonly Mock<RfcInterop> _interopMock = new Mock<RfcInterop>();

        private delegate void GetStringCallback(IntPtr dataHandle, string name, char[] buffer, uint bufferLength, out uint stringLength, out RfcErrorInfo errorInfo);

        private delegate void GetDateCallback(IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo errorInfo);

        private delegate void GetTimeCallback(IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo errorInfo);

        private delegate void GetXStringCallback(IntPtr dataHandle, string name, byte[] buffer, uint bufferLength, out uint xstringLength, out RfcErrorInfo errorInfo);

        [Theory]
        [InlineData("", (int)SapRfcType.RFCTYPE_STRING)]
        [InlineData("hello", (int)SapRfcType.RFCTYPE_STRING)]
        [InlineData("hello", (int)SapRfcType.RFCTYPE_CHAR)]
        [InlineData("hello", (int)SapRfcType.RFCTYPE_NUM)]
        [InlineData("hello", (int)SapRfcType.RFCTYPE_BCD)]
        public void TryGetRfcValue_String(string value, int type)
        {
            // Arrange
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
                    if (buffer.Length <= 0 || bufferLength <= 0)
                        return;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(resultCodeQueue.Dequeue);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Theory]
        [InlineData(1, (int)SapRfcType.RFCTYPE_INT)]
        [InlineData(2, (int)SapRfcType.RFCTYPE_INT1)]
        [InlineData(3, (int)SapRfcType.RFCTYPE_INT2)]
        public void TryGetRfcValue_Integer(int value, int type)
        {
            // Arrange
            int intValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Theory]
        [InlineData(1, (int)SapRfcType.RFCTYPE_INT8)]
        public void TryGetRfcValue_Long(long value, int type)
        {
            // Arrange
            long longValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetInt8(It.IsAny<IntPtr>(), It.IsAny<string>(), out longValue, out errorInfo));

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Theory]
        [InlineData(1.5, (int)SapRfcType.RFCTYPE_FLOAT)]
        public void TryGetRfcValue_Float(double value, int type)
        {
            // Arrange
            double doubleValue = value;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetFloat(It.IsAny<IntPtr>(), It.IsAny<string>(), out doubleValue, out errorInfo));

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Theory]
        [InlineData(1.1, (int)SapRfcType.RFCTYPE_DECF16)]
        [InlineData(2.2, (int)SapRfcType.RFCTYPE_DECF34)]
        public void TryGetRfcValue_Decimal(decimal value, int type)
        {
            // Arrange
            string stringValue = value.ToString(CultureInfo.InvariantCulture);
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
                    if (buffer.Length <= 0 || bufferLength <= 0)
                        return;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(resultCodeQueue.Dequeue);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Theory]
        [InlineData("20200405", (int)SapRfcType.RFCTYPE_DATE)]
        public void TryGetRfcValue_Date(string value, int type)
        {
            // Arrange
            string stringValue = value;
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetDate(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetDateCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    ei = default;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(RfcResultCode.RFC_OK);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(new DateTime(2020, 04, 05));
        }

        [Theory]
        [InlineData("123456", (int)SapRfcType.RFCTYPE_TIME)]
        public void TryGetRfcValue_Time(string value, int type)
        {
            // Arrange
            string stringValue = value;
            RfcErrorInfo errorInfo;
            _interopMock
                .Setup(x => x.GetTime(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<char[]>(), out errorInfo))
                .Callback(new GetTimeCallback((IntPtr dataHandle, string name, char[] buffer, out RfcErrorInfo ei) =>
                {
                    ei = default;
                    Array.Copy(stringValue.ToCharArray(), buffer, stringValue.Length);
                }))
                .Returns(RfcResultCode.RFC_OK);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(new TimeSpan(12, 34, 56));
        }

        [Theory(Skip = "Needs dynamic buffer length detection.")]
        [InlineData(new object[] { new byte[] { 1, 2, 3 }, (int)SapRfcType.RFCTYPE_BYTE })]
        [InlineData(new object[] { new byte[] { 1, 2, 3 }, (int)SapRfcType.RFCTYPE_XSTRING })]
        public void TryGetRfcValue_Byte(byte[] value, int type)
        {
            // Arrange
            uint byteLength = (uint)value.Length;
            RfcErrorInfo errorInfo;
            _interopMock
               .Setup(x => x.GetXString(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<uint>(), out byteLength, out errorInfo))
               .Callback(new GetXStringCallback((IntPtr dataHandle, string name, byte[] buffer, uint bufferLength, out uint sl, out RfcErrorInfo ei) =>
               {
                   ei = default;
                   sl = byteLength;
                   Array.Copy(value, buffer, value.Length);
               }))
                .Returns(RfcResultCode.RFC_OK);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", (SapRfcType)type, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(value);
        }

        [Fact]
        public void TryGetRfcValue_Null()
        {
            // Arrange

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", SapRfcType.RFCTYPE_NULL, null, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().Be(null);
        }

        [Fact]
        public void TryGetRfcValue_Structure()
        {
            // Arrange
            IntPtr structHandle;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetStructure(It.IsAny<IntPtr>(), It.IsAny<string>(), out structHandle, out errorInfo));

            int intValue = 10;
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            Mock<ISapFieldMetadata> fieldMetadatMock = new Mock<ISapFieldMetadata>();
            fieldMetadatMock.SetupGet(x => x.Name).Returns("TEST");
            fieldMetadatMock.SetupGet(x => x.Type).Returns(SapRfcType.RFCTYPE_INT);
            ISapFieldMetadata fieldMetadata = fieldMetadatMock.Object;

            Mock<ISapMetadataCollection<ISapFieldMetadata>> metadataCollectionMock = new Mock<ISapMetadataCollection<ISapFieldMetadata>>();
            metadataCollectionMock.SetupGet(x => x.Count).Returns(1);
            metadataCollectionMock.SetupGet(x => x[It.Is<int>(i => i == 0)]).Returns(fieldMetadata);
            metadataCollectionMock.Setup(x => x.TryGetValue(It.Is<string>(n => n == "TEST"), out fieldMetadata)).Returns(true);
            metadataCollectionMock.Setup(x => x.GetEnumerator()).Returns(new List<ISapFieldMetadata> { fieldMetadata }.GetEnumerator());

            Mock<ISapTypeMetadata> metadataMock = new Mock<ISapTypeMetadata>();
            metadataMock.SetupGet(x => x.Fields).Returns(metadataCollectionMock.Object);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", SapRfcType.RFCTYPE_STRUCTURE, () => metadataMock.Object, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().BeAssignableTo<IReadOnlyDictionary<string, object>>();

            dynamic dynamicStructure = result;

            ((object)dynamicStructure.Count).Should().Be(1);
            ((object)dynamicStructure.TEST).Should().Be(intValue);
            ((object)dynamicStructure[0]).Should().Be(intValue);

            foreach (dynamic kvp in dynamicStructure)
            {
                ((object)kvp.Key).Should().Be("TEST");
                ((object)kvp.Value).Should().Be(intValue);
            }
        }

        [Fact]
        public void TryGetRfcValue_Table()
        {
            // Arrange
            IntPtr tableHandle;
            RfcErrorInfo errorInfo;
            _interopMock.Setup(x => x.GetTable(It.IsAny<IntPtr>(), It.IsAny<string>(), out tableHandle, out errorInfo));

            uint rowCount = 1;
            _interopMock.Setup(x => x.GetRowCount(It.IsAny<IntPtr>(), out rowCount, out errorInfo));

            _interopMock.Setup(x => x.MoveTo(It.IsAny<IntPtr>(), It.Is<uint>(i => i == 0), out errorInfo));
            _interopMock.Setup(x => x.GetCurrentRow(It.IsAny<IntPtr>(), out errorInfo));

            IntPtr structHandle;
            _interopMock.Setup(x => x.GetStructure(It.IsAny<IntPtr>(), It.IsAny<string>(), out structHandle, out errorInfo));

            int intValue = 10;
            _interopMock.Setup(x => x.GetInt(It.IsAny<IntPtr>(), It.IsAny<string>(), out intValue, out errorInfo));

            _interopMock.Setup(x => x.MoveToFirstRow(It.IsAny<IntPtr>(), out errorInfo));
            var resultCodeQueue = new Queue<RfcResultCode>();
            resultCodeQueue.Enqueue(RfcResultCode.RFC_OK);
            resultCodeQueue.Enqueue(RfcResultCode.RFC_TABLE_MOVE_EOF);
            _interopMock.Setup(x => x.MoveToNextRow(It.IsAny<IntPtr>(), out errorInfo)).Returns(resultCodeQueue.Dequeue);

            Mock<ISapFieldMetadata> fieldMetadatMock = new Mock<ISapFieldMetadata>();
            fieldMetadatMock.SetupGet(x => x.Name).Returns("TEST");
            fieldMetadatMock.SetupGet(x => x.Type).Returns(SapRfcType.RFCTYPE_INT);
            ISapFieldMetadata fieldMetadata = fieldMetadatMock.Object;

            Mock<ISapMetadataCollection<ISapFieldMetadata>> metadataCollectionMock = new Mock<ISapMetadataCollection<ISapFieldMetadata>>();
            metadataCollectionMock.SetupGet(x => x.Count).Returns(1);
            metadataCollectionMock.Setup(x => x.TryGetValue(It.Is<string>(n => n == "TEST"), out fieldMetadata)).Returns(true);

            Mock<ISapTypeMetadata> metadataMock = new Mock<ISapTypeMetadata>();
            metadataMock.SetupGet(x => x.Fields).Returns(metadataCollectionMock.Object);

            // Act
            bool valid = DynamicRfc.TryGetRfcValue(_interopMock.Object, DataHandle, "test", SapRfcType.RFCTYPE_TABLE, () => metadataMock.Object, typeof(object), out object result);

            // Assert
            valid.Should().BeTrue();
            result.Should().BeAssignableTo<IReadOnlyList<object>>();

            dynamic dynamicTable = result;

            ((object)dynamicTable.Count).Should().Be(rowCount);
            ((object)dynamicTable[0].TEST).Should().Be(intValue);

            foreach (dynamic row in dynamicTable)
            {
                ((object)row.TEST).Should().Be(intValue);
            }
        }
    }
}
