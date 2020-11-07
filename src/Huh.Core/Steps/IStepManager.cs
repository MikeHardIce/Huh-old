using System.Collections.Generic;

namespace Huh.Core.Steps 
{
    public interface IStepManager<T> where T : IStepInformation
    {
        void Register (T stepInfo);

        ///<summary>
        /// Gets the steps for a specific keyword. In theory, there could be 
        /// multiple Steps assigned to 1 keyword. That would mean
        /// that those steps process the task independent of each other (asynchroneously)
        ///</summary>
        IList<T> GetStepsFor (string keyword); 

        IList<(string keyword, bool enabled, int currentStepHash)> ListSteps ();

        bool DisableStep (int currentStepHash);

        bool EnableStep (int currentStepHash);

        bool RemoveStep (int currentStepHash);   
    }
}