using System.IO;
using System.Linq;
using System.Threading;
using Huh.Core.Data;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Engine.Tasks;

namespace Huh.Engine.Test.Workers.Steps
{
    public class SourceFileDiscovery : IStep
    {
        public ITaskCollection Process(ITask task, CancellationToken cancellationToken)
        {
            var tasks = new TaskCollection();

            var path = task.Records.Where(m => m.Key == "path").Select(m => m.Content as string).First();
            var filetype = task.Records.Where(m => m.Key == "filetype").Select(m => m.Content as string).First();

            foreach(string directory in Directory.EnumerateDirectories(path))
            {
                if(cancellationToken.IsCancellationRequested)
                  return tasks;

                var dirTask = new Task();

                dirTask.KeyWord = "SourceFileDiscovery";
                dirTask.Records.Add(new Record  {Key = "path", Content = directory});
                dirTask.Records.Add(new Record {Key = "filetype", Content = filetype});

                tasks.Add(dirTask);
            }

            foreach(string file in Directory.EnumerateFiles(path, $"*.{filetype}"))
            {
                if(cancellationToken.IsCancellationRequested)
                  return tasks;

                tasks.Add(new Task("", new Record { Key = "sourcefile", Content = file}));
            }

            return tasks;
        }
    }
}