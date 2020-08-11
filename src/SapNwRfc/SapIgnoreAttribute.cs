using System;

namespace SapNwRfc
{
    /// <summary>
    /// Attribute used to ignore a property from mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SapIgnoreAttribute : Attribute
    {
    }
}
