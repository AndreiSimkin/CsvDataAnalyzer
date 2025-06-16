using System.Collections.Generic;

namespace DataAnalyzer.TaskExtensions;

public static class AsyncEnumerableExtensions
{
    public static IEnumerable<T> ToEnumerable<T>(this IAsyncEnumerator<T> asyncEnumerator)
    {
        while (asyncEnumerator.MoveNextAsync().AsTask().Result) yield return asyncEnumerator.Current;
    }
}