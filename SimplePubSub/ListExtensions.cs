using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.SimplePubSub
{
    public static class ListExtensions
    {
        public static IEnumerable<TValue> ShallowCopy<TValue>
        (this IEnumerable<TValue> enumerable)
        {
            List<TValue> list = new List<TValue>();
            foreach (TValue value in enumerable)
            {
                list.Add(value);
            }
            return list;
        }
    }
}
