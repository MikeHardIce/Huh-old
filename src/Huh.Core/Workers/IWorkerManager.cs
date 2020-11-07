
using Huh.Core.Steps;
using Huh.Core.Tasks;

namespace Huh.Core.Workers
{
    public interface IWorkerManager<S>  where S : IStepInformation
    {
        int MaxWorker { get; set; }

        ///<summary>
        /// The number of current workers also indicates how busy the resource is.
        /// It would probably be good to always have 1 worker, if the resource hasn't been stopped
        /// or closed for new work.
        ///</summary>
        int CurrentWorker { get; }

        // TODO: C# 8 will allow partial implementations in interfaces (similar to Traits)
        //       Use this for a default implementation. Its Busy when there are items in
        //       the task collection and CurrentWorker == MaxWorker
        bool Busy { get; }
        ITaskCollectionManager TaskManager { get; }

        IStepManager<S> StepManager { get; }

        void Start();

        ///<summary>
        /// Will accept new tasks, but won't work on them.
        /// This could be used to let the workers finish there work until
        /// all workers are dead,
        ///</summary>
        void StopWorkingOnNewTasks ();

        void Stop();
    }
}