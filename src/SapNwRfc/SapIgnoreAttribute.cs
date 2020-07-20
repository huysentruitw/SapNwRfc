using System;

namespace SapNwRfc
{
    /// <summary>
    /// Attribute used to ignore a property from mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SapIgnoreAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapIgnoreAttribute"/> class.
        /// </summary>
        public SapIgnoreAttribute()
        {
        }
    }
}
