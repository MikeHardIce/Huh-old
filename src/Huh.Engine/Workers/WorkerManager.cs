
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

            ThreadPool.GetAvailableThreads(out int available, out int availableIO);
            this.logger.LogInformation($"Available Threads: {available}, IO: {availableIO}");

            ThreadPool.GetMinThreads(out int minAvailable, out int minIO);

            this.logger.LogInformation($"Min Available Threads: {minAvailable}, IO: {minIO}");

            ThreadPool.GetMaxThreads(out int maxAvailable, out int maxIO);
            this.logger.LogInformation($"Max Available Threads: {maxAvailable}, IO: {maxIO}");
        }

        ~WorkerManager ()
        {
           Console.WriteLine("Destruct");
        }

        public void Reset ()
        {
            if(!this.isRunning)
            {
               this.workerTokenSouce = new CancellationTokenSource();
               this.managerTokenSource = new CancellationTokenSource();
            }
        }        
        public void Start()
        {
            if(!this.isRunning)
            {
                Run();
            }
        }

        private void Run ()
        {
            if(!this.isRunning)
            {
                this.isRunning = true;
                
                  System.Threading.Tasks.Task.Factory.StartNew(() => {
                  Console.WriteLine("--START MANAGER RUN--");
                  try 
                  {
                    while(this.isRunning && !this.managerTokenSource.Token.IsCancellationRequested)
                    {
                        ManageTasks();
                        Thread.Sleep(100);
                    }
                  }
                  finally 
                  {
                    Console.WriteLine("--END MANAGER RUN--");
                  }
                }, this.managerTokenSource.Token);  
            }
        }

        public void Stop()
        {
            this.isRunning = false;
            this.managerTokenSource.Cancel();
            this.workerTokenSouce.Cancel();
            

            this.workers.Clear();
        }

        public void StopWorkingOnNewTasks()
            => this.isRunning = false;
        

        private void ManageTasks()
        {
            Console.WriteLine("ManageTasks BEGIN");
            AssignTasksToIdleWorkers();
            
            CreateNewWorkerIfNeeded();

            ConsumeCreatedTasksOfAllWorkers();

            RemoveIdleWorkers();
            Console.WriteLine("ManageTasks END");
        }

        private void CreateNewWorkerIfNeeded ()
        {
            if(!this.taskManager.TaskCollection.Empty
                    && !this.managerTokenSource.Token.IsCancellationRequested
                    && CurrentWorker < MaxWorker)
            {
                var worker = new Worker(this.stepManager, this.logger);

                this.workers.Add(worker);

                AssignTaskToWorker(worker);
            }
        }

        private void AssignTasksToIdleWorkers ()
            => this.workers.Where(m => !m.Executing).ToList().ForEach(m => {
              if(this.managerTokenSource.Token.IsCancellationRequested)
                return;

              AssignTaskToWorker(m);
            });

        private void ConsumeCreatedTasksOfAllWorkers ()
            =>  this.workers.ToList().ForEach(m => {

                    if(this.managerTokenSource.Token.IsCancellationRequested)
                      return;

                    this.taskManager.TaskCollection.Consume(m.CreatedTasks);
                });
    
        private void RemoveIdleWorkers ()
        {
            var toBeRemoved = this.workers.Where(m => !m.Executing).ToList();

            toBeRemoved.ForEach(m => 
            {
                if(this.managerTokenSource.Token.IsCancellationRequested)
                  return;

                // Since meanwhile, some tasks could have become idle
                this.taskManager.TaskCollection.Consume(m.CreatedTasks);
                this.workers.Remove(m);
            });
        }

        private void AssignTaskToWorker (IWorker worker)
        {
            ITask task = this.taskManager.TaskCollection.TakeHighestPriorityTask();

            if(task != null && !this.workerTokenSouce.Token.IsCancellationRequested)
              worker.Execute(task, this.workerTokenSouce.Token);
        }
  }
}