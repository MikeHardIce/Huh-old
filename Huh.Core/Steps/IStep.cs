

using System.Collections.Generic;
using Huh.Core.Tasks;

namespace Huh.Core.Steps
{
    public interface IStep 
    {
        ITaskCollection Process (ITask task);
    }
}