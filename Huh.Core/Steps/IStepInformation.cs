

namespace Huh.Core.Steps
{
    public interface IStepInformation
    {
        string Keyword { get; }

        int ExecutionOrder { get; }
        
        IStep CreateStep ();
    }
}