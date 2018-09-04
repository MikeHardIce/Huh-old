

using System.Collections.Generic;
using Huh.Core.Tasks;

namespace Huh.Core.Steps
{
    public interface IStep 
    {
        IList<ITask> Process (ITask task);
    }
}