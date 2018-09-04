
using System.Collections.Generic;

namespace Huh.Core.Tasks 
{
    public interface ITaskQueue
    {
        void Add (ITask task);

        void Add (IList<ITask> tasks);

        ITask TakeFirst ();
    }
}