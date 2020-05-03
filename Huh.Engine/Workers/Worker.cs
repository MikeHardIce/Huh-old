
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

            this.logger = logger;
        }
        public void Execute(ITask task, CancellationToken cancelationToken)
        {
            if(!this.isExecuting)
            {
                this.isExecuting = true;

                var keyword = "";
                
                try
                {
                    keyword = task.KeyWord.Dequeue();
                }
                catch(InvalidOperationException)
                {
                    this.logger.LogInformation($"Task does not have a keyword. Skipping task ...");
                    return;
                }
                

                this.logger.LogInformation($"Start prosessing task \"{task.KeyWord}\"");

                var steps   = this.stepManager.GetStepsFor(keyword);

                if(steps.Count < 1)
                {
                    //TODO: Have an option to use to the next keyword???
                    this.logger.LogWarning($"No suitable steps found for keyword \"{keyword}\"");
                    return;
                }

                this.countdown = new CountdownEvent(steps.Count);

                System.Threading.Tasks.Task.Factory.StartNew(() => {

                    this.countdown.Wait();
                    this.isExecuting = false;
                    this.logger.LogInformation($"Task \"{keyword}\" completed");
                });

                steps.ToList().ForEach(m => {
                    System.Threading.Tasks.Task.Factory.StartNew(() => {
                        //sequentially process steps associated to the same StepInfo
                        m.CreateSteps().ToList().ForEach(step => {
                            // in theory the condition could modify the task,
                            if(step.Condition(task))
                                this.createdTasks.Consume(step.Process((ITask)task.Clone()));  
                        });
                        
                        this.countdown.Signal();
                    }, cancelationToken);
                });
            }              
        }
    }
}