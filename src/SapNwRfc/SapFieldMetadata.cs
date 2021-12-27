using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents SAP RFC field metadata.
    /// </summary>
    public sealed class SapFieldMetadata : ISapFieldMetadata
    {
        private readonly RfcInterop _interop;
        private readonly RfcFieldDescription _fieldDescription;

        internal SapFieldMetadata(RfcInterop interop, RfcFieldDescription fieldDescription)
        {
            _interop = interop;
            _fieldDescription = fieldDescription;
        }

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public string Name => _fieldDescription.Name;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public SapRfcType Type => _fieldDescription.Type;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public uint NucLength => _fieldDescription.NucLength;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public uint NucOffset => _fieldDescription.NucOffset;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public uint UcLength => _fieldDescription.UcLength;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public uint UcOffset => _fieldDescription.UcOffset;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public uint Decimals => _fieldDescription.Decimals;

        /// <inheritdoc cref="ISapFieldMetadata"/>
        public ISapTypeMetadata GetTypeMetadata()
        {
            if (_fieldDescription.TypeDescHandle == IntPtr.Zero)
                return null;

            return new SapTypeMetadata(_interop, _fieldDescription.TypeDescHandle);
        }
    }
}
