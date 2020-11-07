
using System.Collections.Generic;
using System.Linq;
using Huh.Core.Data;
using Huh.Core.Tasks;

namespace Huh.Engine.Tasks
{
    public class Task : ITask
    {
        public string KeyWord { get ; set; }
        public long Priority { get; set; }
        public IList<Record> Records { get; set; } = new List<Record>();

        public Task ()
        {

        }
        
        public Task (string keyWord, Record record)
        {
            KeyWord = keyWord;
            Records.Add(record);
        }

        public object Clone()
            => new Task {
                KeyWord = KeyWord
                , Priority = Priority
                , Records = Records.Select(m => m.Copy()).ToList()
            };
    }
}