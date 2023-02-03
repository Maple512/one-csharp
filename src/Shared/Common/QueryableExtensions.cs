namespace System.Linq;

using Expressions;
using OneI;

#if NET
[StackTraceHidden]
#endif
[DebuggerStepThrough]
internal static class QueryableExtensions
{
    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        int offset,
        int takeCount)
    {
        _ = Check.NotNull(query);

        return query.Skip(offset).Take(takeCount);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        _ = Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        _ = Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> @true,
        Expression<Func<T, bool>> @false)
    {
        _ = Check.NotNull(source);

        return source.Where(condition ? @true : @false);
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> @true,
        Expression<Func<T, int, bool>> @false)
    {
        _ = Check.NotNull(source);

        return source.Where(condition ? @true : @false);
    }
}
