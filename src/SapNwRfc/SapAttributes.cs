using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    public sealed class SapAttributes
    {
        private readonly RfcAttributes _attributes;

        internal SapAttributes(RfcAttributes attributes)
        {
            _attributes = attributes;
        }

        public string Destination => _attributes.Destination;

        public string Host => _attributes.Host;

        public string PartnerHost => _attributes.PartnerHost;

        public string SystemNumber => _attributes.SystemNumber;

        public string SystemId => _attributes.SystemId;

        public string Client => _attributes.Client;

        public string User => _attributes.User;

        public string Language => _attributes.Language;

        public string Trace => _attributes.Trace;

        public string IsoLanguage => _attributes.IsoLanguage;

        public string Codepage => _attributes.Codepage;

        public string PartnerCodepage => _attributes.PartnerCodepage;

        public string RfcRole => _attributes.RfcRole;

        public string Type => _attributes.Type;

        public string PartnerType => _attributes.PartnerType;

        public string SystemRelease => _attributes.SystemRelease;

        public string PartnerSystemRelease => _attributes.PartnerSystemRelease;

        public string PartnerKernelRelease => _attributes.PartnerKernelRelease;

        public string CpicConversionId => _attributes.CpicConversionId;

        public string ProgramName => _attributes.ProgramName;

        public string PartnerBytesPerChar => _attributes.PartnerBytesPerChar;

        public string PartnerSystemCodepage => _attributes.PartnerSystemCodepage;

        public string PartnerIPv4 => _attributes.PartnerIPv4;

        public string PartnerIPv6 => _attributes.PartnerIPv6;
    }
}
