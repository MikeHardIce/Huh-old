
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
    public class WorkerManager : IWorkerManager<IStepInformation>
    {
        private readonly IList<Worker> workers;
        private readonly ITaskCollectionManager taskManager;
        private readonly IStepManager<IStepInformation> stepManager;
        
        private readonly ILogger logger;
        private CancellationTokenSource managerTokenSource;
        private CancellationTokenSource workerTokenSouce;
        private bool isRunning;
        public int MaxWorker { get; set; } = 5;

        public int CurrentWorker => this.workers.Count;

        public bool Busy => CurrentWorker >= MaxWorker 
                            && this.workers.All(m => m.Executing);

        public bool IsRunning => this.isRunning;

        public ITaskCollectionManager TaskManager => this.taskManager;

        public IStepManager<IStepInformation> StepManager => this.stepManager;

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
                this.isRunning = true;
                System.Threading.Tasks.Task.Factory.StartNew(() => {

                while(this.isRunning)
                    ManageTasks();

                }, this.managerTokenSource.Token);
            }
        }

        public void Stop()
        {
            this.isRunning = false;
            
            this.workerTokenSouce.Cancel();
            this.managerTokenSource.Cancel();
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

                AssignTaskToWorker(worker);
            }
        }

        private void AssignTasksToIdleWorkers ()
            => this.workers.Where(m => !m.Executing).ToList().ForEach(m => AssignTaskToWorker(m));

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

        private void AssignTaskToWorker (IWorker worker)
        {
            ITask task = this.taskManager.TaskCollection.TakeHighestPriorityTask();

            if(task != null)
              worker.Execute(task, this.workerTokenSouce.Token);
        }
    }
}