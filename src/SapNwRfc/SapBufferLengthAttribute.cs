using System;

namespace SapNwRfc
{
    /// <summary>
    /// Attribute used to set the real SAP RFC property name of a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SapBufferLengthAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapBufferLengthAttribute"/> class.
        /// </summary>
        /// <param name="bufferLength">Length for the byte array buffer.</param>
        public SapBufferLengthAttribute(int bufferLength)
        {
            BufferLength = bufferLength;
        }

        /// <summary>
        /// Gets the length for the byte array buffer.
        /// </summary>
        public int BufferLength { get; }
    }
}
