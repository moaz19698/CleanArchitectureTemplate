namespace Common.Linq
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> WhereIf<T>(
            this IEnumerable<T> query,
            bool IfCondition,
           Func<T, bool> filter)
            where T : class
        {
            if (IfCondition && filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }
        #region distinct By
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = [];
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
        #endregion

    }

}

