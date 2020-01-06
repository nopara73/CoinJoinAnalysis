using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(this IEnumerable<T> items, int len)
        {
            return (len == 1) ?
                items.Select(item => new[] { item }) :
                items.SelectMany((item, i) => items.Skip(i + 1)
                    .CombinationsWithoutRepetition(len - 1)
                    .Select(result => new T[] { item }.Concat(result)));
        }

        public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(this IEnumerable<T> items, int low, int high)
        {
            return Enumerable.Range(low, high).SelectMany(len => items.CombinationsWithoutRepetition(len));
        }

        public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(this IEnumerable<T> items)
        {
            return items.CombinationsWithoutRepetition(1, items.Count());
        }

        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            T current = default;
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (new Random().Next(count) == 0)
                {
                    current = element;
                }
            }
            if (count == 0)
            {
                return default;
            }
            return current;
        }
    }
}
