using System.Collections.Generic;

namespace Huh.Core.Steps 
{
    public interface IStepManager<T> where T : IStepInformation
    {
        void Register (T stepInfo);

        IList<T> GetStepsFor (string keyword); 
        
    }
}