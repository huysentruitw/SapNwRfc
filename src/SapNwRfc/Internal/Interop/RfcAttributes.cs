using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcAttributes
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64 + 1)]
        public string Destination;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string Host;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string PartnerHost;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string SystemNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string SystemId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3 + 1)]
        public string Client;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12 + 1)]
        public string User;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string Language;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Trace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string IsoLanguage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string Codepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string RfcRole;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string SystemRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerSystemRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerKernelRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string CpicConversionId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 + 1)]
        public string ProgramName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerBytesPerChar;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerSystemCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15 + 1)]
        public string PartnerIPv4;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 45 + 1)]
        public string PartnerIPv6;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string Reserved;
    }
}
