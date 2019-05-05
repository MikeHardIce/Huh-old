
namespace Huh.Core.Tasks
{
    public interface ITaskCollectionExt<T> : ITaskCollection
    {
        void Add (T collection);
    }
}