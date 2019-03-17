
using System.Collections.Generic;

namespace Huh.Core.Tasks 
{
    public interface ITaskCollection
    {
        void Add (ITask task);

        void Add (IList<ITask> tasks);

        ITask TakeHighestPriorityTask (string keyword);
    }
}