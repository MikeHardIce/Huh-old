

using System.Collections.Generic;

namespace Huh.Core.Steps
{
    public interface IStepInformation
    {
        string Keyword { get; }

        IList<IStep> CreateSteps ();
    }
}