using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents SAP RFC type metadata.
    /// </summary>
    public sealed class SapTypeMetadata : ISapTypeMetadata
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _typeDescription;

        internal SapTypeMetadata(RfcInterop interop, IntPtr typeDescription)
        {
            _interop = interop;
            _typeDescription = typeDescription;
        }

        /// <inheritdoc cref="ISapTypeMetadata"/>
        public ISapFieldMetadata GetFieldByIndex(uint index)
        {
            RfcResultCode resultCode = _interop.GetFieldDescByIndex(
                typeHandle: _typeDescription,
                index: index,
                fieldDesc: out RfcFieldDescription fieldDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapFieldMetadata(_interop, fieldDesc);
        }

        /// <inheritdoc cref="ISapTypeMetadata"/>
        public ISapFieldMetadata GetFieldByName(string name)
        {
            RfcResultCode resultCode = _interop.GetFieldDescByName(
                typeHandle: _typeDescription,
                name: name,
                fieldDesc: out RfcFieldDescription fieldDesc,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapFieldMetadata(_interop, fieldDesc);
        }

        /// <inheritdoc cref="ISapTypeMetadata"/>
        public uint GetFieldCount()
        {
            RfcResultCode resultCode = _interop.GetFieldCount(
                typeHandle: _typeDescription,
                count: out uint count,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return count;
        }

        /// <inheritdoc cref="ISapTypeMetadata"/>
        public string GetTypeName()
        {
            RfcResultCode resultCode = _interop.GetTypeName(
                rfcHandle: _typeDescription,
                typeName: out string typeName,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return typeName;
        }
    }
}
