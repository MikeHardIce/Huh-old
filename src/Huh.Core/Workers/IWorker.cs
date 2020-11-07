
using System.Threading;
using Huh.Core.Tasks;

namespace Huh.Core.Workers
{
    public interface IWorker
    {
        bool Executing { get; }

        ITaskCollection CreatedTasks { get; }
        void Execute (ITask task, CancellationToken cancelationToken);
    }
}