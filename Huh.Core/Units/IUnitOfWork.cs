
using System.Collections.Generic;
using Huh.Core.Data;

namespace Units
{
    public interface IUnitOfWork
    {
        string KeyWord { get; set; }

        IList<IData<object>> Data { get; set; }
    }
}