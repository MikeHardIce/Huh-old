
using System.Collections.Generic;
using Huh.Core.Steps;

namespace Huh.Engine.Test.Workers.Steps
{
    public class SourceFileDiscoveryInfo : IStepInformation
    {
        public string Keyword => "SourceFileDiscovery";

        public IList<IStep> CreateSteps()
            => new List<IStep> {new SourceFileDiscovery()};
    }
}