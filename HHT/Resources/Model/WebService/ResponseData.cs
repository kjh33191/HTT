
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HHT.Resources.Model
{
    class ResponseData
    {
        public string status { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public Dictionary<string, string> GetDataObject()
        {
            if (data.GetType() == typeof(JObject))
            {
                return ((JObject)data).ToObject<Dictionary<string, string>>();
            }
            else if (data.GetType() == typeof(JArray))
            {
                return ((JArray)data).ToObject<Dictionary<string, string>>();
            }

            return default(Dictionary<string, string>);
        }

        public T GetDataObject<T>()
        {
            if (data.GetType() == typeof(JObject))
            {
                return ((JObject)data).ToObject<T>();
            }
            else if (data.GetType() == typeof(JArray))
            {
                return ((JArray)data).ToObject<T>();
            }

            return default(T);
        }
    }
}