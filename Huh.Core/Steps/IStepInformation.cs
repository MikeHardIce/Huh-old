

namespace Huh.Core.Steps
{
    public interface IStepInformation
    {
        string Keyword { get; }

        IStep CreateStep ();
    }
}