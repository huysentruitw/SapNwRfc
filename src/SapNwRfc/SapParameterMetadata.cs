using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents SAP RFC parameter metadata.
    /// </summary>
    public sealed class SapParameterMetadata : ISapParameterMetadata
    {
        private readonly RfcInterop _interop;
        private readonly RfcParameterDescription _parameterDescription;

        internal SapParameterMetadata(RfcInterop interop, RfcParameterDescription parameterDescription)
        {
            _interop = interop;
            _parameterDescription = parameterDescription;
        }

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public string Name => _parameterDescription.Name;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public SapRfcType Type => _parameterDescription.Type;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public SapRfcDirection Direction => _parameterDescription.Direction;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public uint NucLength => _parameterDescription.NucLength;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public uint UcLength => _parameterDescription.UcLength;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public uint Decimals => _parameterDescription.Decimals;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public string DefaultValue => _parameterDescription.DefaultValue;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public bool Optional => _parameterDescription.Optional == 1;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public string Description => _parameterDescription.ParameterText;

        /// <inheritdoc cref="ISapParameterMetadata"/>
        public ISapTypeMetadata GetTypeMetadata()
        {
            if (_parameterDescription.TypeDescHandle == IntPtr.Zero)
                return null;

            return new SapTypeMetadata(_interop, _parameterDescription.TypeDescHandle);
        }
    }
}
