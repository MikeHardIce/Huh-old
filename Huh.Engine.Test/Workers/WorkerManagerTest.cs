
using System.Threading;
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

            //TODO: Check out alternatives to this
            Thread.Sleep(500);

            manager.TaskManager.TaskCollection.Empty.ShouldBeTrue();
        }
    }
}