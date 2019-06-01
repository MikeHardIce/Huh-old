
using System.Collections.Generic;
using Huh.Core.Steps;
using Huh.Core.Tasks;
using Huh.Core.Workers;
using Huh.Engine.Steps;
using Huh.Engine.Tasks;

namespace Huh.Engine.Workers
{
    public class WorkerManager : IWorkerManager<IStepInformation, TaskCollection>
    {
        private readonly IList<IWorker> workers;
        private readonly ITaskCollectionManager<TaskCollection> taskManager;
        private readonly IStepManager<IStepInformation> stepManager;
        public int MaxWorker { get; set; } = 5;

        public int CurrentWorker => this.workers.Count;

        public bool Busy => CurrentWorker >= MaxWorker 
                            && !TaskManager.TaskCollection.Empty;

        public ITaskCollectionManager<TaskCollection> TaskManager => this.taskManager;

        public IStepManager<IStepInformation> StepManager => this.stepManager;

        public WorkerManager ()
        {
            this.stepManager    = new StepManager();
            this.taskManager    = new TaskCollectionManager();
            this.workers        = new List<IWorker>();
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void StopWorkingOnNewTasks()
        {
            
        }
    }
}