using System;
using System.Collections.Generic;

namespace StorEvil.Utility
{
    static class EnumerableHelpers
    {
        public static IEnumerable<int> UpTo(this int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return i;
            }
        }

        public static void Times(this int times, Action action)
        {
            for (int i = 0; i < times; i++)
            {
                action();
            }
        }

        public static void Each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var x in list)
            {
                action(x);
            }            
        }
    }
}