using System;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC server function.
    /// </summary>
    public sealed class SapServerFunction : ISapServerFunction
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _functionHandle;

        internal SapServerFunction(
            RfcInterop interop,
            IntPtr functionHandle,
            string name)
        {
            _interop = interop;
            _functionHandle = functionHandle;
            Name = name;
        }

        /// <inheritdoc cref="ISapServerFunction"/>
        public string Name { get; }

        /// <inheritdoc cref="ISapServerFunction"/>
        public TOutput GetParameters<TOutput>()
        {
            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }

        /// <inheritdoc cref="ISapServerFunction"/>
        public void SetResult(object input)
        {
            InputMapper.Apply(_interop, _functionHandle, input);
        }
    }
}
