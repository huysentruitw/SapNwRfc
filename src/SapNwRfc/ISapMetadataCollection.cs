using System.Collections.Generic;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for a collection of SAP metadata.
    /// </summary>
    /// <typeparam name="T">The metadata type.</typeparam>
    public interface ISapMetadataCollection<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Tries to get metadata by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The metadata.</param>
        /// <returns><c>true</c> if the metadata was found; otherwise <c>false</c>.</returns>
        bool TryGetValue(string name, out T value);
    }
}
