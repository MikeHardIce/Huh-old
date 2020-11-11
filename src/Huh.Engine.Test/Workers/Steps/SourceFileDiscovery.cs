using System.IO;
using System.Linq;
using Huh.Core.Data;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Engine.Tasks;

namespace Huh.Engine.Test.Workers.Steps
{
    public class SourceFileDiscovery : IStep
    {
        public ITaskCollection Process(ITask task)
        {
            var tasks = new TaskCollection();

            string path = task.Records.Where(m => m.Key == "path").Select(m => m.Content).FirstOrDefault();
            string filetype = task.Records.Where(m => m.Key == "filetype").Select(m => m.Content).FirstOrDefault();

            foreach(string directory in Directory.EnumerateDirectories(path))
            {
                var dirTask = new Task();

                dirTask.KeyWord = "SourceFileDiscovery";
                dirTask.Records.Add(new Record  {Key = "path", Content = directory});
                dirTask.Records.Add(new Record {Key = "filetype", Content = filetype});

                tasks.Add(dirTask);
            }

            foreach(string file in Directory.EnumerateFiles(path, $"*.{filetype}"))
            {
                tasks.Add(new Task("", new Record { Key = "sourcefile", Content = file}));
            }

            return tasks;
        }
    }
}