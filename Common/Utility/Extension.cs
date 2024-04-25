namespace Common.Utility;

public static class Extension
{
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static bool NotNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source != null && source.Any();
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    public static T SelectRandom<T>(this IList<T> listT)
    {
        var random = new Random();
        var index = random.Next(0, listT.Count);

        return listT[index];
    }
}