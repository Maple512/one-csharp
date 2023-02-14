namespace System.Linq;

using Expressions;
using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class QueryableExtensions
{
    public static IQueryable<T> PageBy<T>(
        this IQueryable<T> query,
        int offset,
        int takeCount)
    {
        Check.ThrowIfNull(query);

        return query.Skip(offset).Take(takeCount);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        Check.ThrowIfNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        Check.ThrowIfNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> @true,
        Expression<Func<T, bool>> @false)
    {
        Check.ThrowIfNull(source);

        return source.Where(condition ? @true : @false);
    }

    public static IQueryable<T> WhereIfElse<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, int, bool>> @true,
        Expression<Func<T, int, bool>> @false)
    {
        Check.ThrowIfNull(source);

        return source.Where(condition ? @true : @false);
    }
}
