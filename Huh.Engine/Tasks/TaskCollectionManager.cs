
using System.Collections.Generic;
using Huh.Core.Tasks;

namespace Huh.Engine.Tasks
{
    public class TaskCollectionManager : ITaskCollectionManager
    {
        private readonly ITaskCollection taskCollection;
        public ITaskCollection TaskCollection => this.taskCollection;

        public TaskCollectionManager ()
        {
            this.taskCollection = new TaskCollection();
        }
    }
}