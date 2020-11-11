
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Huh.Core.Data;
using Huh.Engine.Steps;
using Huh.Engine.Tasks;
using Huh.Engine.Test.Workers.Steps;
using Huh.Engine.Workers;
using NUnit.Framework;
using Serilog;
using Serilog.Extensions.Logging;
using Shouldly;

namespace Huh.Engine.Test.Workers
{
    public class OverallTest
    {
        private static readonly List<string> directories = new (){"bla"
                                                                  , Path.Combine("bla","test1")
                                                                  , Path.Combine("bla", "test2")
                                                                  , Path.Combine("bla","test3")
                                                                  , Path.Combine("bla","test3", "test4")};

        private static readonly List<string> files = new () {Path.Combine("bla","Test1.csv")
                                                              , Path.Combine("bla","Test2.csv")
                                                              , Path.Combine("bla","Test3.csv")
                                                              , Path.Combine("bla", "test1","Test11.csv")
                                                              , Path.Combine("bla", "test1","Test12.csv")
                                                              , Path.Combine("bla", "test1","Test13.csv")
                                                              , Path.Combine("bla", "test2","Test21.csv")
                                                              , Path.Combine("bla", "test3", "test4","Test341.ccc")};

        private static readonly string current = Directory.GetCurrentDirectory();

        [OneTimeSetUp]
        public void SetUp ()
        {

           Clean();
           
           directories.ForEach(m => Directory.CreateDirectory(Path.Combine(current, m)));
           
           files.ForEach(m => File.Create(Path.Combine(current, m)));
        } 

        [OneTimeTearDown]
        public void Clean ()
        {
            if (Directory.Exists(current))
                 Directory.Delete(current, recursive: true);
        }
          
        

        [TestCase("", "csv", "Test1.csv", "Test2.csv", "Test3.csv", "Test11.csv", "Test12.csv", "Test13.csv", "Test21.csv")]
        public void TestFileSample (string path, string fileType, params string[] expectedFileNames)
        {
            using var logFactory =  new SerilogLoggerFactory(new LoggerConfiguration()
                                    .WriteTo.Console()
                                    .CreateLogger());

            var manager = new WorkerManager(logFactory.CreateLogger("WorkerManager"));

            try 
            {
                var final = new ResultInfo();

                var task = new Task();

                task.KeyWord = "SourceFileDiscovery";
                task.Records.Add(new Record { Key = "path", Content = Path.Combine(current,path)});
                task.Records.Add(new Record { Key = "filetype", Content = fileType});
                
                
                manager.StepManager.Register(new SourceFileDiscoveryInfo());
                manager.StepManager.Register(final);

                manager.TaskManager.TaskCollection.Add(task);

                manager.Start();

                while (final.Results.IsEmpty || !manager.TaskManager.TaskCollection.Empty) { }
               
                var names = final.Results.ToList();

                expectedFileNames.ToList().All(expected => names.Any(r => r.Content == expected)).ShouldBeTrue();
            }
            finally
            {
                manager.Stop();
            }
        }
    }
}