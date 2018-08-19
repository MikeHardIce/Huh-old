using System.Collections.Generic;

namespace Huh.Core.Steps 
{
    public interface IStepManager 
    {
        void Register (IStepInformation stepInfo);

        IList<IStepInformation> GetBy (string keyword); 
        
    }
}