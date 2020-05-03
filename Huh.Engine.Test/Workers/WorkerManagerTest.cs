
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Huh.Core.Data;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Engine.Tasks;
using Huh.Engine.Workers;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Huh.Engine.Test.Workers 
{
    public class WorkerManagerTest 
    {
        public WorkerManagerTest ()
        {
            
        }

        [Fact]
        public void TestStartStop ()
        {
            var stepInfo = new Mock<IStepInformation>();

            stepInfo.SetupGet(m => m.Keyword).Returns("Hah");

            var task     = new Task();

            task.KeyWord.Enqueue("Hah");

            var manager = new WorkerManager(new Mock<ILogger>().Object);

            manager.StepManager.Register(stepInfo.Object);

            manager.TaskManager.TaskCollection.Add(task);

            manager.Start();
            manager.IsRunning.ShouldBeTrue();

            //TODO: Check out alternatives to this
            Thread.Sleep(500);

            manager.TaskManager.TaskCollection.Empty.ShouldBeTrue();

            manager.Stop();

            manager.IsRunning.ShouldBeFalse();
        }

        [Fact]
        public void TestChaining ()
        {
            var one = new Mock<IStepInformation>();
            var two = new Mock<IStepInformation>();
            var three = new Mock<IStepInformation>();
            
            one.SetupGet(m => m.Keyword).Returns("AddOne");
            two.SetupGet(m => m.Keyword).Returns("AddTwo");
            three.SetupGet(m => m.Keyword).Returns("AddThree");

            one.Setup(m => m.CreateSteps()).Returns(new List<IStep> {
                new Step(1, "AddTwo")
            });

            two.Setup(m => m.CreateSteps()).Returns(new List<IStep> {
                new Step(2, "AddThree")
            });

            three.Setup(m => m.CreateSteps()).Returns(new List<IStep> {
                new Step(3)
            });

            var manager = new WorkerManager(new Mock<ILogger>().Object);

            manager.StepManager.Register(one.Object);
            manager.StepManager.Register(two.Object);
            manager.StepManager.Register(three.Object);

            manager.TaskManager.TaskCollection.Add(new Task("AddOne", new Core.Data.Record("","", 0)));

            manager.Start();

            Thread.Sleep(2000);

            manager.Stop();

            var results = manager.TaskManager.TaskCollection.TakeAll();

            results.Count.ShouldBe(1);

            ITask task = results.FirstOrDefault();

            task.ShouldNotBeNull();

            task.Records.Count.ShouldBe(1);

            Core.Data.Record rec = task.Records.FirstOrDefault();

            Assert.Equal(rec.Content, 6);
        }

        public class Step : IStep
        {
            private readonly int add;
            private readonly string next;
            public Step(int add, string next = "")
            {
                this.add = add;
                this.next = next;
            }
            public ITaskCollection Process(ITask task)
            {
                var collection = new TaskCollection();

                var newTask = (ITask)task.Clone();

                newTask.KeyWord.Clear();

                if(!string.IsNullOrWhiteSpace(this.next))
                    newTask.KeyWord.Enqueue(this.next);

                var record = newTask.Records.FirstOrDefault();
                record.ShouldNotBeNull();
                newTask.Records.Clear();

                newTask.Records.Add(new Core.Data.Record(record.Key, record.ContentHint, record.Content + this.add));
                
                collection.Add(newTask);
                            
                return collection;
            }
        }
    }
}