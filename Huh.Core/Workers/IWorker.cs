
using System.Threading;

namespace Huh.Core.Workers
{
    public interface IWorker
    {
        void Execute (CancellationToken cancelationToken);
    }
}