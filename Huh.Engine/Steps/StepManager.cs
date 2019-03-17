
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
            => PerformAction(currentStepHash, (m) => { m.enabled = false; });

        public bool EnableStep(int currentStepHash)
            => PerformAction(currentStepHash, (m) => { m.enabled = true; });  

        public IList<IStepInformation> GetStepsFor(string keyword)
            => this.steps.Select(m => m.step).Where(m => m.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)).ToList();

        public IList<(string keyword, bool enabled, int currentStepHash)> ListSteps()
            => this.steps.Select(m => (m.step.Keyword, m.enabled, m.step.GetHashCode())).ToList();

        public void Register(IStepInformation stepInfo)
            => this.steps.Add((false, stepInfo));

        public bool RemoveStep(int currentStepHash)
            => PerformAction(currentStepHash, (m) => { this.steps.Remove(m); }); 

        private bool PerformAction (int currentStepHash, Action<(bool enabled, IStepInformation step)> action)
        {
            var step = this.steps.FirstOrDefault(m => m.step.GetHashCode() == currentStepHash);

            if(step.step != null)
            {
                action(step);
                return true;
            }
            
            return false;
        }
    }
}