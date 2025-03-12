using System.Diagnostics.CodeAnalysis;

namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP RFC data types.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Should match SAP SDK")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Should match SAP SDK")]
    public enum SapRfcType
    {
        RFCTYPE_CHAR = 0,
        RFCTYPE_DATE = 1,
        RFCTYPE_BCD = 2,
        RFCTYPE_TIME = 3,
        RFCTYPE_BYTE = 4,
        RFCTYPE_TABLE = 5,
        RFCTYPE_NUM = 6,
        RFCTYPE_FLOAT = 7,
        RFCTYPE_INT = 8,
        RFCTYPE_INT2 = 9,
        RFCTYPE_INT1 = 10,
        RFCTYPE_NULL = 14,
        RFCTYPE_ABAPOBJECT = 16,
        RFCTYPE_STRUCTURE = 17,
        RFCTYPE_DECF16 = 23,
        RFCTYPE_DECF34 = 24,
        RFCTYPE_XMLDATA = 28,
        RFCTYPE_STRING = 29,
        RFCTYPE_XSTRING = 30,
        RFCTYPE_INT8 = 31,
        RFCTYPE_UTCLONG = 32,
        RFCTYPE_UTCSECOND = 33,
        RFCTYPE_UTCMINUTE = 34,
        RFCTYPE_DTDAY = 35,
        RFCTYPE_DTWEEK = 36,
        RFCTYPE_DTMONTH = 37,
        RFCTYPE_TSECOND = 38,
        RFCTYPE_TMINUTE = 39,
        RFCTYPE_CDAY = 40,
        RFCTYPE_BOX = 41,
        RFCTYPE_GENERIC_BOX = 42,
    }
}
