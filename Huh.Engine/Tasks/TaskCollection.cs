
using System.Collections.Generic;
using System.Collections.Concurrent;
using Huh.Core.Tasks;
using System.Linq;
using System;

namespace Huh.Engine.Tasks
{
    public class TaskCollection : ITaskCollection
    {
        private readonly List<ITask> tasks;

        private static object LockGetTask = new object();

        public TaskCollection ()
        {
            this.tasks = new List<ITask>();
        }
        public void Add(ITask task)
            => this.tasks.Add(task);
    

        public void Add(IList<ITask> tasks)
            => this.tasks.AddRange(tasks);
        

        public ITask TakeHighestPriorityTask(string keyword)
        {
            lock(LockGetTask)
            {
                var task = this.tasks.Where(m => m.KeyWord.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(m => m.Priority).FirstOrDefault();

                return task;
            }
        }
    }
}