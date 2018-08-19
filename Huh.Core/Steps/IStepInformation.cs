

namespace Huh.Core.Steps
{
    public interface IStepInformation
    {
        string Name { get; }

        int ExecutionOrder { get; }
        
        IStep CreateStep ();
    }
}