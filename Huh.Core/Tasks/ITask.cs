
using System.Collections.Generic;
using Huh.Core.Data;

namespace Huh.Core.Tasks
{
    public interface ITask
    {
        string KeyWord { get; set; }

        long Priority { get; set; }
        IList<IData<string>> Data { get; set; }
    }
}