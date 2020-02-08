
using System.Collections.Generic;

namespace Huh.Core.Tasks 
{
    public interface ITaskCollection
    {

        bool Empty { get; }
        void Add (ITask task);

        void Add (IList<ITask> tasks);

        ///<summary>
        /// This will remove the tasks from the given TaskCollection
        /// and add it to the collection that consumes the tasks
        ///</summary>
        void Consume (ITaskCollection tasks);

        ///<summary>
        ///Takes all tasks out of the collection
        ///</summary>
        ICollection<ITask> TakeAll ();

        ITask TakeHighestPriorityTask (string keyword);

        ITask TakeHighestPriorityTask();

    }
}