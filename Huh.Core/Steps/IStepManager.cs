using System.Collections.Generic;

namespace Huh.Core.Steps 
{
    public interface IStepManager<T> where T : IStepInformation
    {
        void Register (T stepInfo);

        IList<T> GetStepsFor (string keyword); 

        IList<(string keyword, bool enabled, int currentStepHash)> ListSteps ();

        bool DisableStep (int currentStepHash);

        bool EnableStep (int currentStepHash);

        bool RemoveStep (int currentStepHash);   
    }
}