
using System.Collections.Generic;
using Huh.Core.Tasks;

namespace Huh.Engine.Tasks
{
    public class TaskCollectionManager : ITaskCollectionManager<TaskCollection>
    {
        private readonly ITaskCollectionExt<TaskCollection> taskCollection;
        public ITaskCollectionExt<TaskCollection> TaskCollection => this.taskCollection;

        public TaskCollectionManager ()
        {
            this.taskCollection = new TaskCollection();
        }
    }
}