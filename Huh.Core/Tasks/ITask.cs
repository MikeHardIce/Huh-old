
using System;
using System.Collections.Generic;
using Huh.Core.Data;

namespace Huh.Core.Tasks
{
    public interface ITask : ICloneable
    {
        Queue<string> KeyWord { get; set; }

        long Priority { get; set; }
        IList<Record> Records { get; set; }       
    }
}