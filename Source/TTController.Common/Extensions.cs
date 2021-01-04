using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TTController.Common
{
    public static class Extensions
    {
        public static bool ContentsEqual<T>(this IList<T> first, IList<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null) return second == null;
            if (second == null) return false;
            if (ReferenceEquals(first, second)) return true;
            if (first.Count != second.Count) return false;
            if (first.Count == 0) return true;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < first.Count; i++)
                if (!comparer.Equals(first[i], second[i]))
                    return false;

            return true;
        }

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
            => task.IsCompleted ? task : task.ContinueWith(completedTask => completedTask.GetAwaiter().GetResult(),
                                                           cancellationToken,
                                                           TaskContinuationOptions.ExecuteSynchronously,
                                                           TaskScheduler.Default);

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static void Deconstruct<TKey, TElement>(this IGrouping<TKey, TElement> group, out TKey key, out IEnumerable<TElement> elements)
        {
            key = group.Key;
            elements = group.AsEnumerable();
        }

        public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> enumberable, int rotate)
            => enumberable.Skip(rotate).Concat(enumberable.Take(rotate));

        public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> enumberable, int rotate)
            => enumberable.RotateLeft(enumberable.Count() - rotate);
    }
}
