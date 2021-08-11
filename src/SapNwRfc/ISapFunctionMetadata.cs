using System.Collections.Generic;

namespace SapNwRfc
{
    /// <summary>
    /// Interface for SAP RFC function metadata.
    /// </summary>
    public interface ISapFunctionMetadata
    {
        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <returns>The function name.</returns>
        string GetName();

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        IReadOnlyList<ISapParameterMetadata> Parameters { get; }

        /// <summary>
        /// Gets parameter metadata by name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter metadata.</returns>
        ISapParameterMetadata GetParameterByName(string name);

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        IReadOnlyList<ISapExceptionMetadata> Exceptions { get; }

        /// <summary>
        /// Gets exception metadata by name.
        /// </summary>
        /// <param name="name">The name of the exception.</param>
        /// <returns>The exception metadata.</returns>
        ISapExceptionMetadata GetExceptionByName(string name);
    }
}
