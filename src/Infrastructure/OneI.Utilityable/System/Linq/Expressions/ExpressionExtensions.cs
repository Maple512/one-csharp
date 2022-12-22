namespace System.Linq.Expressions;
/// <summary>
/// The expression extensions.
/// </summary>

[StackTraceHidden]
[DebuggerStepThrough]
public static class ExpressionExtensions
{
    /// <summary>
    /// Ands the.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>An Expression.</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.AndAlso);
    }

    /// <summary>
    /// Ors the.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>An Expression.</returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.OrElse);
    }

    /// <summary>
    /// Combines the lambdas.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <param name="expressionType">The expression type.</param>
    /// <returns>An Expression.</returns>
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

    /// <summary>
    /// The substitute parameter visitor.
    /// </summary>
    private class SubstituteParameterVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Sub = new();

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An Expression.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Sub.TryGetValue(node, out var newValue) ? newValue : node;
        }
    }
}
