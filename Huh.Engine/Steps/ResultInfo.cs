
using System.Collections.Concurrent;
using System.Collections.Generic;
using Huh.Core.Data;
using Huh.Core.Steps;

namespace Huh.Engine.Steps
{
  public class ResultInfo : IStepInformation
  {
      private ConcurrentQueue<Record> results;
      public string Keyword => "";

      public ConcurrentQueue<Record> Results => this.results;

      public ResultInfo ()
        => this.results = new ConcurrentQueue<Record>();

      public IList<IStep> CreateSteps()
        => new List<IStep> {new Results(this.results)};
  }
}