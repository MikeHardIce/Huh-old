using Huh.Engine.Steps;
using Huh.Core.Steps;
using Xunit;
using Moq;
using System.Collections.Generic;
using Shouldly;

namespace Huh.Engine.Test.Steps
{
    public class StepManagerTest
    {
        private readonly StepManager stepManager;
        public StepManagerTest ()
        {
            this.stepManager = new StepManager();
        }

        [Fact]
        public void TestBasicRegistration ()
        {
            var stepInfo = new Mock<IStepInformation>();
            stepInfo.SetupGet(m => m.Keyword).Returns("abc");
            
            this.stepManager.Register(stepInfo.Object);
            IList<IStepInformation> steps = this.stepManager.GetStepsFor("abc");

            steps.Count.ShouldBe(1);
        }


    }
}