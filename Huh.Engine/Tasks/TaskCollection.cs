
using System.Collections.Generic;
using Huh.Core.Tasks;
using System.Linq;
using System;

namespace Huh.Engine.Tasks
{
    public class TaskCollection : ITaskCollection
    {
        private readonly List<ITask> tasks;

        public bool Empty => this.tasks.Count < 1;

        private object LockGetTask = new object();

        public TaskCollection ()
        {
            this.tasks = new List<ITask>();
        }
        public void Add(ITask task)
            => this.tasks.Add(task);
    

        public void Add(IList<ITask> tasks)
            => this.tasks.AddRange(tasks);

        public void Consume(ITaskCollection collection)
        {
            while(!collection.Empty)
            {
                Add(collection.TakeHighestPriorityTask());
            }
        }

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