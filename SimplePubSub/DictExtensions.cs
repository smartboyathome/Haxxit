using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartboyDevelopments.SimplePubSub.ExtensionMethods
{
    public static class DictExtensions
    {
        // GetValueOrDefault copied from:
        // http://stackoverflow.com/questions/2601477/dictionary-returning-a-default-value-if-the-key-does-not-exist
        // They have been modified to better suit what I needed.

        public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
         TKey key,
         TValue defaultValue)
        {
            TValue value;
            bool success = dictionary.TryGetValue(key, out value);
            if (!success)
            {
                value = defaultValue;
                dictionary.Add(key, value);
            }
            return value;
        }

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             Func<TKey, TValue> defaultValueProvider)
        {
            TValue value;
            bool success = dictionary.TryGetValue(key, out value);
            if(!success)
            {
                value = defaultValueProvider(key);
                dictionary.Add(key, value);
            }
            return value;
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> GetAllMatchingRegex<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             string regular_expression)
        {
            if (!typeof(TKey).IsAssignableFrom(typeof(string)))
                return new List<KeyValuePair<TKey, TValue>>();
            return from item in dictionary
                   where Regex.Match(item.Key.ToString(), regular_expression).Success
                   select item;
        }
    }
}
