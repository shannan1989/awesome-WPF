using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Shannan.Core.Extensions
{
    public static class JTokenExtension
    {
        public static List<string> Keys(this JObject self)
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, JToken> item in self)
            {
                keys.Add(item.Key);
            }
            return keys;
        }
    }
}
