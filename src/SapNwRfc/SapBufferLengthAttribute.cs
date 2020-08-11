using System;

namespace SapNwRfc
{
    /// <summary>
    /// Attribute used to set the SAP RFC byte buffer length on a byte array property.
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
