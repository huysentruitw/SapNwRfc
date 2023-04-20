using System;
using SapNwRfc.Internal.Fields;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Dynamic
{
    internal static class DynamicRfc
    {
        internal static bool TryGetRfcValue(RfcInterop interop, IntPtr dataHandle, string name, SapRfcType type, Func<ISapTypeMetadata> typeMetadata, Type returnType, out object result)
        {
            result = GetRfcValue(interop, dataHandle, name, type, typeMetadata);

            if (returnType == typeof(object))
            {
                return true;
            }

            try
            {
                result = Convert.ChangeType(result, returnType);
                return true;
            }
            catch { }

            result = null;
            return false;
        }

        private static object GetRfcValue(RfcInterop interop, IntPtr dataHandle, string name, SapRfcType type, Func<ISapTypeMetadata> typeMetadata)
        {
            switch (type)
            {
            case SapRfcType.RFCTYPE_CHAR:
            case SapRfcType.RFCTYPE_NUM:
            case SapRfcType.RFCTYPE_BCD:
            case SapRfcType.RFCTYPE_STRING:
                return StringField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_INT:
            case SapRfcType.RFCTYPE_INT1:
            case SapRfcType.RFCTYPE_INT2:
                return IntField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_INT8:
                return LongField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_FLOAT:
                return DoubleField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_DECF16:
            case SapRfcType.RFCTYPE_DECF34:
                return DecimalField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_DATE:
                return DateField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_TIME:
                return TimeField.Extract(interop, dataHandle, name).Value;

            case SapRfcType.RFCTYPE_BYTE:
            case SapRfcType.RFCTYPE_XSTRING:
                return BytesField.Extract(interop, dataHandle, name, bufferLength: 0).Value;

            case SapRfcType.RFCTYPE_TABLE:
                {
                    RfcResultCode resultCode = interop.GetTable(
                        dataHandle: dataHandle,
                        name: name,
                        tableHandle: out IntPtr tableHandle,
                        errorInfo: out RfcErrorInfo errorInfo);

                    resultCode.ThrowOnError(errorInfo);

                    return new DynamicRfcTable(interop, tableHandle, typeMetadata());
                }

            case SapRfcType.RFCTYPE_STRUCTURE:
                {
                    RfcResultCode resultCode = interop.GetStructure(
                        dataHandle: dataHandle,
                        name: name,
                        structHandle: out IntPtr structHandle,
                        errorInfo: out RfcErrorInfo errorInfo);

                    resultCode.ThrowOnError(errorInfo);

                    return new DynamicRfcStructure(interop, structHandle, typeMetadata());
                }

            case SapRfcType.RFCTYPE_NULL:
                return null;
            }

            throw new NotSupportedException($"Parameter type {type} is not supported");
        }
    }
}
