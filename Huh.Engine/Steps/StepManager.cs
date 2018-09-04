
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Huh.Core.Steps;

namespace Huh.Engine.Steps
{
    public class StepManager : IStepManager<IStepInformation>
    {
        private ConcurrentBag<IStepInformation> bagOfSteps;
        public IList<IStepInformation> GetStepsFor(string keyword)
        {
            return this.bagOfSteps.Where(m => m.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public void Register(IStepInformation stepInfo)
        {
            this.bagOfSteps.Add(stepInfo);
        }
    }
}