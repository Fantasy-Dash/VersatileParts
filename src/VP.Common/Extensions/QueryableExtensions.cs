using System.Linq.Expressions;

namespace VP.Common.Extensions
{
    public static class QueryableExtensions
    {
        /// <param name="condition">if value is <see langword="false"/> then return source</param>
        /// <returns>
        /// An <see cref="IQueryable{TSource}"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="predicate"/>
        /// or return source <see cref="IQueryable{TSource}"/>.
        /// </returns>
        /// <inheritdoc cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
        public static IQueryable<TSource> IfWhere<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        /// <param name="condition">if value is <see langword="false"/> then return source</param>
        /// <returns>
        /// An <see cref="IQueryable{TSource}"/> that contains elements from the input sequence that satisfy the condition specified by <paramref name="predicate"/>
        /// or return source <see cref="IQueryable{TSource}"/>.
        /// </returns>
        /// <inheritdoc cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource,int, bool}})"/>
        public static IQueryable<TSource> IfWhere<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, int, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }
}
