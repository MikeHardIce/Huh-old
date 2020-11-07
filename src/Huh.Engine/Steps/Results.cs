
using System.Collections.Concurrent;
using System.Linq;
using Huh.Core.Data;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Engine.Tasks;

namespace Huh.Engine.Steps
{
  public class Results : IStep
  {
      private readonly ConcurrentQueue<Record> results;
      public Results (ConcurrentQueue<Record> results)
        => this.results = results;

      public ITaskCollection Process(ITask task)
      {
        task.Records.ToList().ForEach(m => results.Enqueue(m));
        return new TaskCollection();
      }
  }
}