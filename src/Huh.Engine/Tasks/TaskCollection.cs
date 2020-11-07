
using System.Collections.Generic;
using Huh.Core.Tasks;
using System.Linq;
using System;

namespace Huh.Engine.Tasks
{
    public class TaskCollection : ITaskCollection
    {
        //TODO: Find a better collection, keyword search and ordering will be super slow
        // when the collection gets large
        private List<ITask> tasks;

        public bool Empty => this.tasks.Count < 1;

        private object LockGetTask = new object();

        public TaskCollection ()
            => this.tasks = new List<ITask>();
        
        public void Add(ITask task)
        {
            lock(this.LockGetTask)
            {
                try 
                {
                  this.tasks.Add(task);
                }
                catch (Exception)
                {

                }
            }
        }
    
        public void Add(IList<ITask> tasks)
            => tasks.ToList().ForEach(m => Add(m));

        public void Consume(ITaskCollection collection)
            => Add(collection.TakeAll().OrderByDescending(m => m.Priority).ToList());
            
        public ITask TakeHighestPriorityTask(string keyword)
            => TakeTask(m => m.Where(by => by.KeyWord.Equals(keyword, StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(by => by.Priority).FirstOrDefault());

        public ITask TakeHighestPriorityTask()
            => TakeTask(m => m.OrderByDescending(by => by.Priority).FirstOrDefault());

        public ICollection<ITask> TakeAll()
        {
            lock(LockGetTask)
            {
                var collection = this.tasks;

                this.tasks = new List<ITask>();

                return collection;
            }
        }

        private ITask TakeTask (Func<IList<ITask>, ITask> func)
        {
            lock(LockGetTask)
            {
                var task = func(this.tasks);

                if(task != null)
                    this.tasks.Remove(task);

                return task;
            }
        }
    }
}