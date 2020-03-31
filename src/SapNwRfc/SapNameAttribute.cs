using System;

namespace SapNwRfc
{
    /// <summary>
    /// Attribute used to set the real SAP RFC property name of a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SapNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The SAP RFC property name.</param>
        public SapNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the SAP RFC property name.
        /// </summary>
        public string Name { get; }
    }
}
