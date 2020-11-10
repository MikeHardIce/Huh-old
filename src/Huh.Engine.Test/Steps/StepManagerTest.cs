using Huh.Engine.Steps;
using Huh.Core.Steps;
using Moq;
using System.Collections.Generic;
using Shouldly;
using System.Linq;
using NUnit.Framework;

namespace Huh.Engine.Test.Steps
{
    public class StepManagerTest
    {
        private StepManager stepManager;


        [SetUp]
        public void SetUp ()
        {
            this.stepManager = new StepManager();

            var stepInfo = new Mock<IStepInformation>();
            stepInfo.SetupGet(m => m.Keyword).Returns("abc");
            
            this.stepManager.Register(stepInfo.Object);
        }

        [Test]
        public void TestBasicRegistration ()
        {   
            IList<IStepInformation> steps = this.stepManager.GetStepsFor("abc");

            steps.Count.ShouldBe(1);

            steps.Any(m => m.Keyword == "abc").ShouldBeTrue();

            var allSteps = this.stepManager.ListSteps();

            var step = allSteps.FirstOrDefault();

            step.enabled.ShouldBeFalse();
            step.keyword.ShouldBe("abc");
            step.currentStepHash.ShouldNotBe(0);

            this.stepManager.RemoveStep(step.currentStepHash);

            this.stepManager.ListSteps().Count.ShouldBe(0);
        }

        [Test]
        public void TestEnablingDisablingStep ()
        {
            this.stepManager.ListSteps().Any(m => m.enabled).ShouldBeFalse();

            var step = this.stepManager.ListSteps().FirstOrDefault();

            this.stepManager.EnableStep(step.currentStepHash);

            this.stepManager.ListSteps().Any(m => m.enabled).ShouldBeTrue();

            this.stepManager.DisableStep(step.currentStepHash);

            this.stepManager.ListSteps().Any(m => m.enabled).ShouldBeFalse();
        }
    }
}