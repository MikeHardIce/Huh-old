
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Core.Workers;
using Huh.Engine.Tasks;

namespace Huh.Engine.Workers
{
    public class Worker : IWorker
    {
        private readonly IStepManager<IStepInformation> stepManager;

        private readonly ITaskCollection createdTasks;

        private CountdownEvent countdown;
        private bool isExecuting;
        public bool Executing => this.isExecuting;

        public ITaskCollection CreatedTasks => this.createdTasks;

        public Worker (IStepManager<IStepInformation> stepManager)
        {
            this.stepManager = stepManager;

            this.createdTasks = new TaskCollection();
        }
        public void Execute(ITask task, CancellationToken cancelationToken)
        {
            if(!this.isExecuting)
            {
                this.isExecuting = true;

                var steps = this.stepManager.GetStepsFor(task.KeyWord);

                if(steps.Count < 1)
                {
                    // TODO: maybe some kind of logging
                    return;
                }

                this.countdown = new CountdownEvent(steps.Count);


                steps.ToList().ForEach(m => {
                    Task.Factory.StartNew(() => {
                        
                        m.CreateSteps().ToList().ForEach(step => {
                            this.createdTasks.Add(step.Process(task));
                        });
                        
                        this.countdown.Signal();
                    }, cancelationToken);
                });

                var waitingTask = Task.Factory.StartNew(() => {

                    this.countdown.Wait();
                    this.isExecuting = false;
                });
 
            }
              
        }
    }
}