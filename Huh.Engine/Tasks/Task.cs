
using System.Collections.Generic;
using System.Linq;
using Huh.Core.Data;
using Huh.Core.Tasks;

namespace Huh.Engine.Tasks
{
    public class Task : ITask
    {
        public Queue<string> KeyWord { get ; set; } = new Queue<string>();
        public long Priority { get; set; }
        public IList<IData<string>> Data { get; set; } = new List<IData<string>>();

        public Task ()
        {

        }
        
        public Task (string keyWord, SimpleData data)
        {
            KeyWord.Enqueue(keyWord);
            Data.Add(data);
        }

        public object Clone()
            => new Task {
                KeyWord = new Queue<string>(KeyWord)
                , Priority = Priority
                , Data = (IList<IData<string>>) Data.Select(m => new SimpleData { Data = m.Data
                            , Key = m.Key
                            , ContentHint = m.ContentHint
                            , ContentType = m.ContentType 
                            }).ToList()
            };
    }
}