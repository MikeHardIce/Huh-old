
using System.Threading;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Engine.Workers;
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
            var task     = new Mock<ITask>();

            var manager = new WorkerManager();

            manager.StepManager.Register(stepInfo.Object);

            manager.TaskManager.TaskCollection.Add(task.Object);

            manager.Start();

            //TODO: Check out alternatives to this
            Thread.Sleep(500);

            manager.TaskManager.TaskCollection.Empty.ShouldBeTrue();
            
        }
    }
}