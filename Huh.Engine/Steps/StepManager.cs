
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Huh.Core.Steps;

namespace Huh.Engine.Steps
{
    public class StepManager : IStepManager<IStepInformation>
    {
        private readonly IList<(bool enabled, IStepInformation step)> steps;

        public StepManager()
        {
            this.steps = new List<(bool enabled, IStepInformation step)>();
        }
        public bool DisableStep(int currentStepHash)
            => PerformAction(currentStepHash, m => { m.enabled = false; return true; });

        public bool EnableStep(int currentStepHash)
            => PerformAction(currentStepHash, m => { m.enabled = true; return true; });  

        public IList<IStepInformation> GetStepsFor(string keyword)
            => this.steps.Select(m => m.step).Where(m => m.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)).ToList();

        public IList<(string keyword, bool enabled, int currentStepHash)> ListSteps()
            => this.steps.Select(m => (m.step.Keyword, m.enabled, m.step.GetHashCode())).ToList();

        public void Register(IStepInformation stepInfo)
            => this.steps.Add((false, stepInfo));

        public bool RemoveStep(int currentStepHash)
            => PerformAction(currentStepHash, m => { return this.steps.Remove(m); }); 

        private bool PerformAction (int currentStepHash, Func<(bool enabled, IStepInformation step), bool> action)
        {
            var step = this.steps.FirstOrDefault(m => m.step.GetHashCode() == currentStepHash);

            if(step.step != null)
            {
                return action(step);
            }
            
            return false;
        }
    }
}