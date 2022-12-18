namespace System.Linq;

using System.Linq.Expressions;
using OneI;

#if NET7_0_OR_GREATER
[StackTraceHidden]
#endif
[DebuggerStepThrough]
internal static class QueryableExtensions
{
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="offset">偏移量</param>
    /// <param name="takeCount">拾取的数量</param>
    /// <returns></returns>
    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        int offset,
        int takeCount)
    {
        Check.NotNull(query);

        return query.Skip(offset).Take(takeCount);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> @true,
        Expression<Func<T, bool>> @false)
    {
        Check.NotNull(source);

        return source.Where(condition ? @true : @false);
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> @true,
        Expression<Func<T, int, bool>> @false)
    {
        Check.NotNull(source);

        return source.Where(condition ? @true : @false);
    }
}
