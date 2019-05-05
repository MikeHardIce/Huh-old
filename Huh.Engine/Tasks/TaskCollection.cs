
using System.Collections.Generic;
using System.Collections.Concurrent;
using Huh.Core.Tasks;
using System.Linq;
using System;

namespace Huh.Engine.Tasks
{
    public class TaskCollection : ITaskCollectionExt<TaskCollection>
    {
        private readonly List<ITask> tasks;

        protected List<ITask> Tasks => this.tasks;
        private object LockGetTask = new object();

        public TaskCollection ()
        {
            this.tasks = new List<ITask>();
        }
        public void Add(ITask task)
            => this.tasks.Add(task);
    

        public void Add(IList<ITask> tasks)
            => this.tasks.AddRange(tasks);

        public void Add(TaskCollection collection)
            => Add(collection.Tasks);

        public ITask TakeHighestPriorityTask(string keyword)
            => TakeTask(m => m.Where(by => by.KeyWord.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(by => by.Priority).FirstOrDefault());

        public ITask TakeHighestPriorityTask()
            => TakeTask(m => m.OrderByDescending(by => by.Priority).FirstOrDefault());

        private ITask TakeTask (Func<IList<ITask>, ITask> func)
        {
            lock(LockGetTask)
            {
                var task = func(this.tasks);

                this.tasks.Remove(task);

                return task;
            }
        }

        
    }
}