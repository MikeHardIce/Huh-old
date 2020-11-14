
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Core.Workers;
using Huh.Engine.Tasks;
using Microsoft.Extensions.Logging;

namespace Huh.Engine.Workers
{
    public class Worker : IWorker, IDisposable
    {
        private readonly IStepManager<IStepInformation> stepManager;

        private readonly ITaskCollection createdTasks;

        private readonly ILogger logger;

        private CountdownEvent countdown;
        private bool isExecuting;
        public bool Executing => this.isExecuting;

        public ITaskCollection CreatedTasks => this.createdTasks;

        public Worker (IStepManager<IStepInformation> stepManager, ILogger logger)
        {
            this.stepManager = stepManager;

            this.createdTasks = new TaskCollection();

            this.logger = logger;
        }
        public void Execute(ITask task, CancellationToken cancelationToken)
        {
            if(!this.isExecuting && !cancelationToken.IsCancellationRequested)
            {
                this.isExecuting = true;
                
                this.logger.LogInformation($"Start prosessing task \"{task.KeyWord}\"");

                var steps   = this.stepManager.GetStepsFor(task.KeyWord);

                if(steps.Count < 1)
                {
                    //TODO: Have an option to use to the next keyword???
                    this.logger.LogWarning($"No suitable steps found for keyword \"{task.KeyWord}\"");
                    return;
                }

                var allSteps = steps.SelectMany(m => m.CreateSteps()).ToList();
                this.countdown = new CountdownEvent(allSteps.Count);
                Console.WriteLine("Signals: " + allSteps.Count);
                System.Threading.Tasks.Task.Factory.StartNew(() => {

                    this.countdown.Wait();
                    this.isExecuting = false;
                    this.logger.LogInformation($"Task \"{task.KeyWord}\" completed");
                }, cancelationToken);

                allSteps.ForEach(step => System.Threading.Tasks.Task.Factory.StartNew(() => {
                    // in theory the condition could modify the task,
                    try
                    {
                        if(step.Condition(task) && !cancelationToken.IsCancellationRequested)
                        {
                            this.logger.LogInformation($"Process {task.KeyWord} with {task.Records.Count} records.");
                            this.createdTasks.Consume(step.Process((ITask)task.Clone(), cancelationToken));
                        }
                    }
                    finally 
                    {
                        this.countdown.Signal();
                        Console.WriteLine("Signal");
                    }
                }, cancelationToken));
            }              
        }

    public void Dispose()
    {
      
    }
  }
}