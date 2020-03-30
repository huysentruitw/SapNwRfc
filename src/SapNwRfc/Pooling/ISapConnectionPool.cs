using System;
using System.Threading;

namespace SapNwRfc.Pooling
{
    public interface ISapConnectionPool : IDisposable
    {
        ISapConnection GetConnection(CancellationToken cancellationToken = default);

        void ReturnConnection(ISapConnection connection);

        void ForgetConnection(ISapConnection connection);
    }
}
