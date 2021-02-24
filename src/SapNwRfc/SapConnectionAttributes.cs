using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    public sealed class SapConnectionAttributes
    {
        private readonly RfcAttributes _attributes;

        internal SapConnectionAttributes(RfcAttributes attributes)
        {
            _attributes = attributes;
        }

        public string Dest => _attributes.Dest;

        public string Host => _attributes.Host;

        public string PartnerHost => _attributes.PartnerHost;

        public string SysNumber => _attributes.SysNumber;

        public string SysId => _attributes.SysId;

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

        public string Rel => _attributes.Rel;

        public string PartnerRel => _attributes.PartnerRel;

        public string KernelRel => _attributes.KernelRel;

        public string CpicConvId => _attributes.CpicConvId;

        public string ProgName => _attributes.ProgName;

        public string PartnerBytesPerChar => _attributes.PartnerBytesPerChar;

        public string PartnerSystemCodepage => _attributes.PartnerSystemCodepage;

        public string PartnerIP => _attributes.PartnerIP;

        public string PartnerIPv6 => _attributes.PartnerIPv6;
    }
}
