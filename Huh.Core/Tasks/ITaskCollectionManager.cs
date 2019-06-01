
namespace Huh.Core.Tasks
{
    public interface ITaskCollectionManager<T> where T : ITaskCollection
    {
        ITaskCollectionExt<T> TaskCollection { get; }

    }
}