using System.Collections.Generic;
using System.Linq;

namespace RedditerCore.Utilities
{
    public static class StringExtension
    {
        public static List<KeyValuePair<string, string>> SplitToKeyValuePairs(this string str, char pairSeparator, char keyValueSeparator)
        {
            return str.Split(pairSeparator)
                .Select(pair => pair.Split(keyValueSeparator))
                .Select(kv => new KeyValuePair<string, string>(kv[0], kv[1]))
                .ToList();
        }

        public static Dictionary<string, string> SplitToDictionary(this string str, char pairSeparator, char keyValueSeparator)
        {
            return str.Split(pairSeparator)
                .Select(pair => pair.Split(keyValueSeparator))
                .ToDictionary(kv => kv[0], kv => kv[1]);
        }
    }
}
