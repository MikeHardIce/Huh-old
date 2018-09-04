
using System.Collections.Generic;
using Huh.Core.Data;

namespace Huh.Core.Tasks
{
    public interface ITask
    {
        string KeyWord { get; set; }

        IList<IData<object>> Data { get; set; }
    }
}