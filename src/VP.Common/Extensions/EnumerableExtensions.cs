namespace VP.Common.Extensions
{
    /// <summary>
    /// Enumerable 扩展
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <param name="condition">if value is <see langword="false"/> then return source</param>
        /// <returns>
        /// An <see cref="IEnumerable{TSource}"/> that contains elements from the input sequence that satisfy the condition
        /// or return source <see cref="IEnumerable{TSource}"/>.
        /// </returns>
        /// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        public static IEnumerable<TSource> IfWhere<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        /// <param name="condition">if value is <see langword="false"/> then return source</param>
        /// <returns>
        /// An <see cref="IEnumerable{TSource}"/> that contains elements from the input sequence that satisfy the condition
        /// or return source <see cref="IEnumerable{TSource}"/>.
        /// </returns>
        /// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource,int, bool})"/>
        public static IEnumerable<TSource> IfWhere<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }
}
