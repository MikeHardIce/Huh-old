
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Core.Workers;
using Huh.Engine.Steps;
using Huh.Engine.Tasks;
using Microsoft.Extensions.Logging;

namespace Huh.Engine.Workers
{
    public class FinalTaskEventArgs : EventArgs
    {
        public ITask FinalTask { get; set; }
    } 
    public class WorkerManager : IWorkerManager<IStepInformation>
    {
        private readonly IList<Worker> workers;
        private readonly ITaskCollectionManager taskManager;
        private readonly IStepManager<IStepInformation> stepManager;
        
        private readonly ILogger logger;
        private CancellationTokenSource managerTokenSource;
        private CancellationTokenSource workerTokenSouce;
        public bool isRunning;
        public int MaxWorker { get; set; } = 5;

        public int CurrentWorker => this.workers.Count;

        public bool Busy => CurrentWorker >= MaxWorker 
                            && this.workers.All(m => m.Executing);

        public ITaskCollectionManager TaskManager => this.taskManager;

        public IStepManager<IStepInformation> StepManager => this.stepManager;

        public EventHandler<FinalTaskEventArgs> FinalTaskEvent {get; set;}

        public WorkerManager (ILogger logger)
        {
            this.stepManager        = new StepManager();
            this.taskManager        = new TaskCollectionManager();
            this.workers            = new List<Worker>();

            this.managerTokenSource = new CancellationTokenSource();
            this.workerTokenSouce   = new CancellationTokenSource();

            this.logger             = logger;
        }
        
        public void Start()
        {
            if(!this.isRunning)
            {
                this.isRunning = true;
                if(this.workerTokenSouce.IsCancellationRequested)
                {
                    this.workerTokenSouce = new CancellationTokenSource();
                }

                if(this.managerTokenSource.IsCancellationRequested)
                {
                    this.managerTokenSource = new CancellationTokenSource();
                }

                Run();
            }
        }

        private void Run ()
        {
            if(!this.isRunning)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() => {

                    ManageTasks();
                    Thread.SpinWait(100);
                    Run();
       
                }, this.managerTokenSource.Token);
            }
        }

        public void Stop()
        {
            this.isRunning = false;
            
            this.workerTokenSouce.Cancel();
        }

        public void StopWorkingOnNewTasks()
            => this.isRunning = false;
        

        private void ManageTasks()
        {
            AssignTasksToIdleWorkers();
            
            CreateNewWorkerIfNeeded();

            ConsumeCreatedTasksOfAllWorkers();

            RemoveIdleWorkers();
        }

        private void CreateNewWorkerIfNeeded ()
        {
            if(!this.taskManager.TaskCollection.Empty
                    && CurrentWorker < MaxWorker)
            {
                var worker = new Worker(this.stepManager, this.logger);

                this.workers.Add(worker);

                worker.Execute(this.taskManager.TaskCollection.TakeHighestPriorityTask(), this.workerTokenSouce.Token);
            }
        }

        private void AssignTasksToIdleWorkers ()
            => this.workers.Where(m => !m.Executing).ToList().ForEach(m => {
                    ITask task = this.taskManager.TaskCollection.TakeHighestPriorityTask();

                    if(task != null)
                    {
                        try 
                        {
                            if(task.KeyWord.Count > 0 && task.KeyWord.Peek().Length > 1)
                            {
                                m.Execute(task, this.workerTokenSouce.Token);
                            }
                            else 
                            {
                                FinalTaskEvent(this, new FinalTaskEventArgs { FinalTask = task});
                            }
                        }
                        catch(InvalidOperationException)
                        {
                            this.logger.LogInformation("Found task without a keyword. Skipping task ...");
                        }
                    }
                });

        private void ConsumeCreatedTasksOfAllWorkers ()
            =>  this.workers.ToList().ForEach(m => {
                    this.taskManager.TaskCollection.Consume(m.CreatedTasks);
                });
    
        private void RemoveIdleWorkers ()
        {
            var toBeRemoved = this.workers.Where(m => !m.Executing).ToList();

            toBeRemoved.ForEach(m => 
            {
                // Since meanwhile, some tasks could have become idle
                this.taskManager.TaskCollection.Consume(m.CreatedTasks);
                this.workers.Remove(m);
            });
        }
    }
}