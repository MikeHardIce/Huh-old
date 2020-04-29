

using System.Collections.Generic;
using Huh.Core.Tasks;

namespace Huh.Core.Steps
{
    public interface IStep 
    {
        bool Condition (ITask task) => true; // would be nice to somehow disallow modifications, since it could be a reference type
        ITaskCollection Process (ITask task);
    }
}