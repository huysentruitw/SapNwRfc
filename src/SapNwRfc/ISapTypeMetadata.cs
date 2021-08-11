using System.Collections.Generic;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC type metadata.
    /// </summary>
    public interface ISapTypeMetadata
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        string GetTypeName();

        /// <summary>
        /// Gets the fields.
        /// </summary>
        IReadOnlyList<ISapFieldMetadata> Fields { get; }

        /// <summary>
        /// Gets field metadata by name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>The field metadata.</returns>
        ISapFieldMetadata GetFieldByName(string name);
    }
}
