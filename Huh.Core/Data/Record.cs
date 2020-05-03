
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Huh.Core.Data
{
    public struct Record
    {
        public string Key;

        ///<summary>
        /// Any kind of hint that helps you to identify what the content
        /// of the data is.
        ///</summary>
        public string ContentHint;
        public dynamic Content;

        public Record(string key, string contentHint, dynamic content)
        {
            Key = key;
            ContentHint = contentHint;
            Content = content;           
        }

        public Record Copy ()
            => new Record(Key
                    , ContentHint
                    , JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(Content), new ExpandoObjectConverter()));
        
            
        
    }
}