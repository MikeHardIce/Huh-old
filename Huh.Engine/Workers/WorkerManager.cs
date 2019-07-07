
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Core.Workers;
using Huh.Engine.Steps;
using Huh.Engine.Tasks;

namespace Huh.Engine.Workers
{
    public class WorkerManager : IWorkerManager<IStepInformation>
    {
        private readonly IList<Worker> workers;
        private readonly ITaskCollectionManager taskManager;
        private readonly IStepManager<IStepInformation> stepManager;
        
        private CancellationTokenSource managerTokenSource;
        private CancellationTokenSource workerTokenSouce;
        public bool started;
        public int MaxWorker { get; set; } = 5;

        public int CurrentWorker => this.workers.Count;

        public bool Busy => CurrentWorker >= MaxWorker 
                            && this.workers.All(m => m.Executing);

        public ITaskCollectionManager TaskManager => this.taskManager;

        public IStepManager<IStepInformation> StepManager => this.stepManager;

        public WorkerManager ()
        {
            this.stepManager        = new StepManager();
            this.taskManager        = new TaskCollectionManager();
            this.workers            = new List<Worker>();
            this.managerTokenSource = new CancellationTokenSource();
            this.workerTokenSouce   = new CancellationTokenSource();
        }
        public void Start()
        {
            if(!this.started)
            {
                this.started = true;
                if(this.workerTokenSouce.IsCancellationRequested)
                {
                    this.workerTokenSouce = new CancellationTokenSource();
                }

                if(this.managerTokenSource.IsCancellationRequested)
                {
                    this.managerTokenSource = new CancellationTokenSource();
                }

                Task.Factory.StartNew(() => {

                    ManageTasks();
                    Thread.SpinWait(100);
       
                }, this.managerTokenSource.Token);
            }
        }

        public void Stop()
        {
            this.started = false;
            
            this.workerTokenSouce.Cancel();
        }

        public void StopWorkingOnNewTasks()
        {
            this.started = false;
        }

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
                var worker = new Worker(this.stepManager);

                this.workers.Add(worker);

                worker.Execute(this.taskManager.TaskCollection.TakeHighestPriorityTask(), this.workerTokenSouce.Token);
            }
        }

        private void AssignTasksToIdleWorkers ()
        {
            this.workers.Where(m => !m.Executing).ToList().ForEach(m => {
                ITask task = this.taskManager.TaskCollection.TakeHighestPriorityTask();

                if(task != null && task.KeyWord.Length > 1)
                {
                    m.Execute(task, this.workerTokenSouce.Token);
                }
            });

        }

        private void ConsumeCreatedTasksOfAllWorkers ()
        {
            this.workers.ToList().ForEach(m => {
                this.taskManager.TaskCollection.Consume(m.CreatedTasks);
            });
        }

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