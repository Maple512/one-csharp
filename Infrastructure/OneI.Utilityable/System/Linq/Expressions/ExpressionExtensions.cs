namespace System.Linq.Expressions;

using System.Collections.Generic;
using System.Diagnostics;

[StackTraceHidden]
[DebuggerStepThrough]
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.AndAlso);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.OrElse);
    }

    public static Expression<Func<T, bool>> CombineLambdas<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        ExpressionType expressionType)
    {
        var visitor = new SubstituteParameterVisitor
        {
            Sub =
            {
                [right.Parameters[0]] = left.Parameters[0],
            },
        };

        var body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));

        return Expression.Lambda<Func<T, bool>>(body, left.Parameters[0]);
    }

    private class SubstituteParameterVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Sub = new();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Sub.TryGetValue(node, out var newValue) ? newValue : node;
        }
    }
}
