using System;
using System.Threading;

namespace SapNwRfc.Pooling
{
    public interface ISapPooledConnection : IDisposable
    {
        void InvokeFunction(string name, CancellationToken cancellationToken = default);

        void InvokeFunction(string name, object input, CancellationToken cancellationToken = default);

        TOutput InvokeFunction<TOutput>(string name, CancellationToken cancellationToken = default);

        TOutput InvokeFunction<TOutput>(string name, object input, CancellationToken cancellationToken = default);
    }
}
