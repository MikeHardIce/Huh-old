
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
    public class Worker : IWorker
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
        }
        public void Execute(ITask task, CancellationToken cancelationToken)
        {
            if(!this.isExecuting)
            {
                this.logger.LogInformation($"Start prosessing task \"{task.KeyWord}\"");
                this.isExecuting = true;

                var steps = this.stepManager.GetStepsFor(task.KeyWord);

                if(steps.Count < 1)
                {
                    this.logger.LogWarning($"No suitable steps found for keyword \"{task.KeyWord}\"");
                    return;
                }

                this.countdown = new CountdownEvent(steps.Count);

                Task.Factory.StartNew(() => {

                    this.countdown.Wait();
                    this.isExecuting = false;
                    this.logger.LogInformation($"Task \"{task.KeyWord}\" completed");
                });

                steps.ToList().ForEach(m => {
                    Task.Factory.StartNew(() => {
                        //sequentially process steps associated to the same StepInfo
                        m.CreateSteps().ToList().ForEach(step => {
                            
                            this.createdTasks.Add(step.Process(task));
                        });
                        
                        this.countdown.Signal();
                    }, cancelationToken);
                });

            }
              
        }
    }
}