
using Huh.Core.Data;

namespace Huh.Engine.Data
{
    public struct SimpleData : IData<string>
    {
        public string Key { get; set; }
        public DataContent ContentType { get; set; }
        public string ContentHint { get; set; }
        public string Data { get; set; }
    }
}