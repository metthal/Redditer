using System.Collections.Generic;
using System.Text;

namespace RedditerCore.Utilities
{
    public static class EnumerableKeyValuePairExtension
    {
        public static string AsQueryString(this IEnumerable<KeyValuePair<string, string>> list)
        {
            var builder = new StringBuilder();
            foreach (var keyValue in list)
            {
                builder.AppendFormat("{0}={1}&", keyValue.Key, keyValue.Value);
            }
            return builder.ToString();
        }
    }
}
