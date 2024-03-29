namespace System.Linq;

using Expressions;
using Reflection;

/// <summary>
/// source: https://github.com/dotnet/efcore/blob/3a62379ec775eb8e6a6ef7334212005b80faadb7/src/EFCore/Query/ExpressionPrinter.cs
/// </summary>
public class ExpressionPrinter : ExpressionVisitor
{
    private static readonly List<string> SimpleMethods = new()
    {
        "get_Item",
        "TryReadValue",
        "ReferenceEquals"
    };

    private readonly IndentedStringBuilder _stringBuilder;
    private readonly Dictionary<ParameterExpression, string?> _parametersInScope;
    private readonly List<ParameterExpression> _namelessParameters;
    private readonly List<ParameterExpression> _encounteredParameters;

    private readonly Dictionary<ExpressionType, string> _binaryOperandMap = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " == " },
        { ExpressionType.NotEqual, " != " },
        { ExpressionType.GreaterThan, " > " },
        { ExpressionType.GreaterThanOrEqual, " >= " },
        { ExpressionType.LessThan, " < " },
        { ExpressionType.LessThanOrEqual, " <= " },
        { ExpressionType.OrElse, " || " },
        { ExpressionType.AndAlso, " && " },
        { ExpressionType.Coalesce, " ?? " },
        { ExpressionType.Add, " + " },
        { ExpressionType.Subtract, " - " },
        { ExpressionType.Multiply, " * " },
        { ExpressionType.Divide, " / " },
        { ExpressionType.Modulo, " % " },
        { ExpressionType.And, " & " },
        { ExpressionType.Or, " | " },
        { ExpressionType.ExclusiveOr, " ^ " }
    };

    /// <summary>
    ///     Creates a new instance of the <see cref="ExpressionPrinter" /> class.
    /// </summary>
    public ExpressionPrinter()
    {
        _stringBuilder = new IndentedStringBuilder();
        _parametersInScope = new Dictionary<ParameterExpression, string?>();
        _namelessParameters = new List<ParameterExpression>();
        _encounteredParameters = new List<ParameterExpression>();
    }

    /// <summary>
    /// Gets or sets the character limit.
    /// </summary>
    private int? CharacterLimit { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether verbose.
    /// </summary>
    private bool Verbose { get; set; }

    /// <summary>
    ///     Appends a new line to current output being built.
    /// </summary>
    /// <returns>This printer so additional calls can be chained.</returns>
    public virtual ExpressionPrinter AppendLine()
    {
        _stringBuilder.AppendLine();
        return this;
    }

    /// <summary>
    ///     Appends the given string and a new line to current output being built.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>This printer so additional calls can be chained.</returns>
    public virtual ExpressionVisitor AppendLine(string value)
    {
        _stringBuilder.AppendLine(value);
        return this;
    }

    /// <summary>
    ///     Appends all the lines to current output being built.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <param name="skipFinalNewline">If true, then a terminating new line is not added.</param>
    /// <returns>This printer so additional calls can be chained.</returns>
    public virtual ExpressionPrinter AppendLines(string value, bool skipFinalNewline = false)
    {
        _stringBuilder.AppendLines(value, skipFinalNewline);
        return this;
    }

    /// <summary>
    ///     Creates a scoped indenter that will increment the indent, then decrement it when disposed.
    /// </summary>
    /// <returns>An indenter.</returns>
    public virtual IDisposable Indent()
    {
        return _stringBuilder.Indent();
    }

    /// <summary>
    ///     Appends the given string to current output being built.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <returns>This printer so additional calls can be chained.</returns>
    public virtual ExpressionPrinter Append(string value)
    {
        _stringBuilder.Append(value);
        return this;
    }

    /// <summary>
    ///     Creates a printable string representation of the given expression.
    /// </summary>
    /// <param name="expression">The expression to print.</param>
    /// <returns>The printable representation.</returns>
    public static string Print(Expression expression)
    {
        return new ExpressionPrinter().PrintCore(expression);
    }

    /// <summary>
    ///     Creates a printable verbose string representation of the given expression.
    /// </summary>
    /// <param name="expression">The expression to print.</param>
    /// <returns>The printable representation.</returns>
    public static string PrintDebug(Expression expression)
    {
        return new ExpressionPrinter().PrintCore(expression, verbose: true);
    }

    /// <summary>
    ///     Creates a printable string representation of the given expression.
    /// </summary>
    /// <param name="expression">The expression to print.</param>
    /// <param name="characterLimit">An optional limit to the number of characters included. Additional output will be truncated.</param>
    /// <returns>The printable representation.</returns>
    public virtual string PrintExpression(Expression expression, int? characterLimit = null)
    {
        return PrintCore(expression, characterLimit);
    }

    /// <summary>
    ///     Creates a printable verbose string representation of the given expression.
    /// </summary>
    /// <param name="expression">The expression to print.</param>
    /// <returns>The printable representation.</returns>
    public virtual string PrintExpressionDebug(Expression expression)
    {
        return PrintCore(expression, verbose: true);
    }

    /// <summary>
    /// Prints the core.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="characterLimit">The character limit.</param>
    /// <param name="verbose">If true, verbose.</param>
    /// <returns>A string.</returns>
    private string PrintCore(Expression expression, int? characterLimit = null, bool verbose = false)
    {
        _stringBuilder.Clear();
        _parametersInScope.Clear();
        _namelessParameters.Clear();
        _encounteredParameters.Clear();

        CharacterLimit = characterLimit;
        Verbose = verbose;

        _ = Visit(expression);

        var queryPlan = PostProcess(_stringBuilder.ToString());

        if(characterLimit is > 0)
        {
            queryPlan = queryPlan.Length > characterLimit
                ? queryPlan[..characterLimit.Value] + "..."
                : queryPlan;
        }

        return queryPlan;
    }

    /// <summary>
    ///     Returns binary operator string corresponding to given <see cref="ExpressionType" />.
    /// </summary>
    /// <param name="expressionType">The expression type to generate binary operator for.</param>
    /// <returns>The binary operator string.</returns>
    public virtual string GenerateBinaryOperator(ExpressionType expressionType)
    {
        return _binaryOperandMap[expressionType];
    }

    /// <summary>
    ///     Visit given readonly collection of expression for printing.
    /// </summary>
    /// <param name="items">A collection of items to print.</param>
    /// <param name="joinAction">A join action to use when joining printout of individual item in the collection.</param>
    public virtual void VisitCollection<T>(IReadOnlyCollection<T> items, Action<ExpressionPrinter>? joinAction = null)
        where T : Expression
    {
        joinAction ??= (p => p.Append(", "));

        var first = true;
        foreach(var item in items)
        {
            if(!first)
            {
                joinAction(this);
            }
            else
            {
                first = false;
            }

            _ = Visit(item);
        }
    }

    /// <inheritdoc />
    [return: NotNullIfNotNull("expression")]
    public override Expression? Visit(Expression? expression)
    {
        if(expression == null)
        {
            return null;
        }

        if(CharacterLimit != null
            && _stringBuilder.Length > CharacterLimit.Value)
        {
            return expression;
        }

        switch(expression.NodeType)
        {
            case ExpressionType.AndAlso:
            case ExpressionType.ArrayIndex:
            case ExpressionType.Assign:
            case ExpressionType.Equal:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.NotEqual:
            case ExpressionType.OrElse:
            case ExpressionType.Coalesce:
            case ExpressionType.Add:
            case ExpressionType.Subtract:
            case ExpressionType.Multiply:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.ExclusiveOr:
                _ = VisitBinary((BinaryExpression)expression);
                break;

            case ExpressionType.Block:
                _ = VisitBlock((BlockExpression)expression);
                break;

            case ExpressionType.Conditional:
                _ = VisitConditional((ConditionalExpression)expression);
                break;

            case ExpressionType.Constant:
                _ = VisitConstant((ConstantExpression)expression);
                break;

            case ExpressionType.Lambda:
                _ = base.Visit(expression);
                break;

            case ExpressionType.Goto:
                _ = VisitGoto((GotoExpression)expression);
                break;

            case ExpressionType.Label:
                _ = VisitLabel((LabelExpression)expression);
                break;

            case ExpressionType.MemberAccess:
                _ = VisitMember((MemberExpression)expression);
                break;

            case ExpressionType.MemberInit:
                _ = VisitMemberInit((MemberInitExpression)expression);
                break;

            case ExpressionType.Call:
                _ = VisitMethodCall((MethodCallExpression)expression);
                break;

            case ExpressionType.New:
                _ = VisitNew((NewExpression)expression);
                break;

            case ExpressionType.NewArrayInit:
                _ = VisitNewArray((NewArrayExpression)expression);
                break;

            case ExpressionType.Parameter:
                _ = VisitParameter((ParameterExpression)expression);
                break;

            case ExpressionType.Convert:
            case ExpressionType.Throw:
            case ExpressionType.Not:
            case ExpressionType.TypeAs:
            case ExpressionType.Quote:
                _ = VisitUnary((UnaryExpression)expression);
                break;

            case ExpressionType.Default:
                _ = VisitDefault((DefaultExpression)expression);
                break;

            case ExpressionType.Try:
                _ = VisitTry((TryExpression)expression);
                break;

            case ExpressionType.Index:
                _ = VisitIndex((IndexExpression)expression);
                break;

            case ExpressionType.TypeIs:
                _ = VisitTypeBinary((TypeBinaryExpression)expression);
                break;

            case ExpressionType.Switch:
                _ = VisitSwitch((SwitchExpression)expression);
                break;

            case ExpressionType.Invoke:
                _ = VisitInvocation((InvocationExpression)expression);
                break;

            case ExpressionType.Extension:
                _ = VisitExtension(expression);
                break;

            default:
                UnhandledExpressionType(expression);
                break;
        }

        return expression;
    }

    /// <inheritdoc />
    protected override Expression VisitBinary(BinaryExpression binaryExpression)
    {
        _ = Visit(binaryExpression.Left);

        if(binaryExpression.NodeType == ExpressionType.ArrayIndex)
        {
            _stringBuilder.Append("[");

            _ = Visit(binaryExpression.Right);

            _stringBuilder.Append("]");
        }
        else
        {
            if(!_binaryOperandMap.TryGetValue(binaryExpression.NodeType, out var operand))
            {
                UnhandledExpressionType(binaryExpression);
            }
            else
            {
                _stringBuilder.Append(operand);
            }

            _ = Visit(binaryExpression.Right);
        }

        return binaryExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitBlock(BlockExpression blockExpression)
    {
        _ = AppendLine();
        _ = AppendLine("{");

        using(_stringBuilder.Indent())
        {
            foreach(var variable in blockExpression.Variables)
            {
                if(!_parametersInScope.ContainsKey(variable))
                {
                    _parametersInScope.Add(variable, variable.Name);
                    _ = Append(variable.Type.Name);
                    _ = Append(" ");
                    _ = VisitParameter(variable);
                    _ = AppendLine(";");
                }
            }

            var expressions = blockExpression.Expressions.Count > 0
                ? blockExpression.Expressions.Except(new[] { blockExpression.Result })
                : blockExpression.Expressions;

            foreach(var expression in expressions)
            {
                _ = Visit(expression);
                _ = AppendLine(";");
            }

            if(blockExpression.Expressions.Count > 0)
            {
                if(blockExpression.Result.Type != typeof(void))
                {
                    _ = Append("return ");
                }

                _ = Visit(blockExpression.Result);
                _ = AppendLine(";");
            }
        }

        _ = Append("}");

        return blockExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
    {
        _ = Visit(conditionalExpression.Test);

        _stringBuilder.Append(" ? ");

        _ = Visit(conditionalExpression.IfTrue);

        _stringBuilder.Append(" : ");

        _ = Visit(conditionalExpression.IfFalse);

        return conditionalExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
        PrintValue(constantExpression.Value);

        return constantExpression;

        void PrintValue(object? value)
        {
            if(value is IEnumerable enumerable
                and not string)
            {
                _stringBuilder.Append(value.GetType().Name + " { ");

                var first = true;
                foreach(var item in enumerable)
                {
                    if(first)
                    {
                        first = false;
                    }
                    else
                    {
                        _stringBuilder.Append(", ");
                    }

                    PrintValue(item);
                }

                _stringBuilder.Append(" }");
                return;
            }

            var stringValue = value == null
                ? "null"
                : value.ToString() != value.GetType().ToString()
                    ? value.ToString()
                    : value.GetType().Name;

            if(value is string)
            {
                stringValue = $@"""{stringValue}""";
            }

            _stringBuilder.Append(stringValue ?? "Unknown");
        }
    }

    /// <inheritdoc />
    protected override Expression VisitGoto(GotoExpression gotoExpression)
    {
        _ = AppendLine("return (" + gotoExpression.Target.Type.Name + ")" + gotoExpression.Target + " {");
        using(_stringBuilder.Indent())
        {
            _ = Visit(gotoExpression.Value);
        }

        _stringBuilder.Append("}");

        return gotoExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitLabel(LabelExpression labelExpression)
    {
        _stringBuilder.Append(labelExpression.Target.ToString());

        return labelExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitLambda<T>(Expression<T> lambdaExpression)
    {
        if(lambdaExpression.Parameters.Count != 1)
        {
            _stringBuilder.Append("(");
        }

        foreach(var parameter in lambdaExpression.Parameters)
        {
            var parameterName = parameter.Name;

            if(!_parametersInScope.ContainsKey(parameter))
            {
                _parametersInScope.Add(parameter, parameterName);
            }

            _ = Visit(parameter);

            if(parameter != lambdaExpression.Parameters.Last())
            {
                _stringBuilder.Append(", ");
            }
        }

        if(lambdaExpression.Parameters.Count != 1)
        {
            _stringBuilder.Append(")");
        }

        _stringBuilder.Append(" => ");

        _ = Visit(lambdaExpression.Body);

        foreach(var parameter in lambdaExpression.Parameters)
        {
            // however we don't remove nameless parameters so that they are unique globally, not just within the scope
            _ = _parametersInScope.Remove(parameter);
        }

        return lambdaExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitMember(MemberExpression memberExpression)
    {
        if(memberExpression.Expression != null)
        {
            if(memberExpression.Expression.NodeType == ExpressionType.Convert
                || memberExpression.Expression is BinaryExpression)
            {
                _stringBuilder.Append("(");
                _ = Visit(memberExpression.Expression);
                _stringBuilder.Append(")");
            }
            else
            {
                _ = Visit(memberExpression.Expression);
            }
        }
        else
        {
            // ReSharper disable once PossibleNullReferenceException
            _stringBuilder.Append(memberExpression.Member.DeclaringType?.Name ?? "MethodWithoutDeclaringType");
        }

        _stringBuilder.Append("." + memberExpression.Member.Name);

        return memberExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
    {
        _stringBuilder.Append("new " + memberInitExpression.Type.Name);

        var appendAction = memberInitExpression.Bindings.Count > 1 ? (Func<string, ExpressionVisitor>)AppendLine : Append;
        _ = appendAction("{ ");
        using(_stringBuilder.Indent())
        {
            for(var i = 0; i < memberInitExpression.Bindings.Count; i++)
            {
                var binding = memberInitExpression.Bindings[i];
                if(binding is MemberAssignment assignment)
                {
                    _stringBuilder.Append(assignment.Member.Name + " = ");
                    _ = Visit(assignment.Expression);
                    _ = appendAction(i == memberInitExpression.Bindings.Count - 1 ? " " : ", ");
                }
                else
                {
                    _ = AppendLine($"Unhandled member binding type '{binding.BindingType}'.");
                }
            }
        }

        _ = AppendLine("}");

        return memberInitExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        if(methodCallExpression.Object != null)
        {
            switch(methodCallExpression.Object)
            {
                case BinaryExpression:
                case UnaryExpression:
                    _stringBuilder.Append("(");
                    _ = Visit(methodCallExpression.Object);
                    _stringBuilder.Append(")");
                    break;
                default:
                    _ = Visit(methodCallExpression.Object);
                    break;
            }

            _stringBuilder.Append(".");
        }

        var methodArguments = methodCallExpression.Arguments.ToList();
        var method = methodCallExpression.Method;

        var extensionMethod = !Verbose
            && methodCallExpression.Arguments.Count > 0
            && method.IsDefined(typeof(ExtensionAttribute), inherit: false);

        if(extensionMethod)
        {
            _ = Visit(methodArguments[0]);
            _ = _stringBuilder.Indent();
            _stringBuilder.AppendLine();
            _stringBuilder.Append($".{method.Name}");
            methodArguments = methodArguments.Skip(1).ToList();
            if(method.Name is nameof(Enumerable.Cast) or nameof(Enumerable.OfType))
            {
                PrintGenericArguments(method, _stringBuilder);
            }
        }
        else
        {
            if(method.IsStatic)
            {
                _stringBuilder.Append(method.DeclaringType!.Name);

                _stringBuilder.Append('.');
            }

            _stringBuilder.Append(method.Name);
            PrintGenericArguments(method, _stringBuilder);
        }

        _stringBuilder.Append("(");

        var isSimpleMethodOrProperty = SimpleMethods.Contains(method.Name)
            || methodArguments.Count < 2;

        var appendAction = isSimpleMethodOrProperty ? (Func<string, ExpressionVisitor>)Append : AppendLine;

        if(methodArguments.Count > 0)
        {
            _ = appendAction("");

            var argumentNames
                = !isSimpleMethodOrProperty
                    ? extensionMethod
                        ? method.GetParameters().Skip(1).Select(p => p.Name).ToList()
                        : method.GetParameters().Select(p => p.Name).ToList()
                    : new List<string?>();

            IDisposable? indent = null;

            if(!isSimpleMethodOrProperty)
            {
                indent = _stringBuilder.Indent();
            }

            for(var i = 0; i < methodArguments.Count; i++)
            {
                var argument = methodArguments[i];

                if(!isSimpleMethodOrProperty)
                {
                    _stringBuilder.Append(argumentNames[i] + ": ");
                }

                _ = Visit(argument);

                if(i < methodArguments.Count - 1)
                {
                    _ = appendAction(", ");
                }
            }

            if(!isSimpleMethodOrProperty)
            {
                indent?.Dispose();
            }
        }

        _ = Append(")");

        if(extensionMethod)
        {
            _ = _stringBuilder.DeIndent();
        }

        return methodCallExpression;

        static void PrintGenericArguments(MethodInfo method, IndentedStringBuilder stringBuilder)
        {
            if(method.IsGenericMethod)
            {
                stringBuilder.Append("<");
                var first = true;
                foreach(var genericArgument in method.GetGenericArguments())
                {
                    if(!first)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(genericArgument.Name);
                    first = false;
                }

                stringBuilder.Append(">");
            }
        }
    }

    /// <inheritdoc />
    protected override Expression VisitNew(NewExpression newExpression)
    {
        _stringBuilder.Append("new ");

        var isComplex = newExpression.Arguments.Count > 1;
        var appendAction = isComplex ? (Func<string, ExpressionVisitor>)AppendLine : Append;

        var isAnonymousType = newExpression.Type.IsAnonymousType();
        if(!isAnonymousType)
        {
            _stringBuilder.Append(newExpression.Type.Name);
            _ = appendAction("(");
        }
        else
        {
            _ = appendAction("{ ");
        }

        IDisposable? indent = null;
        if(isComplex)
        {
            indent = _stringBuilder.Indent();
        }

        for(var i = 0; i < newExpression.Arguments.Count; i++)
        {
            if(newExpression.Members != null)
            {
                _ = Append(newExpression.Members[i].Name + " = ");
            }

            _ = Visit(newExpression.Arguments[i]);
            _ = appendAction(i == newExpression.Arguments.Count - 1 ? "" : ", ");
        }

        if(isComplex)
        {
            indent?.Dispose();
        }

        _stringBuilder.Append(!isAnonymousType ? ")" : " }");

        return newExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitNewArray(NewArrayExpression newArrayExpression)
    {
        var isComplex = newArrayExpression.Expressions.Count > 1;
        var appendAction = isComplex ? s => AppendLine(s) : (Action<string>)(s => Append(s));

        appendAction("new " + newArrayExpression.Type.GetElementType()!.Name + "[]");
        appendAction("{ ");

        IDisposable? indent = null;
        if(isComplex)
        {
            indent = _stringBuilder.Indent();
        }

        VisitArguments(newArrayExpression.Expressions, appendAction, lastSeparator: " ");

        if(isComplex)
        {
            indent?.Dispose();
        }

        _ = Append("}");

        return newArrayExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitParameter(ParameterExpression parameterExpression)
    {
        if(_parametersInScope.TryGetValue(parameterExpression, out var parameterName))
        {
            if(parameterName == null)
            {
                if(!_namelessParameters.Contains(parameterExpression))
                {
                    _namelessParameters.Add(parameterExpression);
                }

                _ = Append("namelessParameter{");
                _ = Append(_namelessParameters.IndexOf(parameterExpression).ToString());
                _ = Append("}");
            }
            else if(parameterName.Contains('.'))
            {
                _ = Append("[");
                _ = Append(parameterName);
                _ = Append("]");
            }
            else
            {
                _ = Append(parameterName);
            }
        }
        else
        {
            if(Verbose)
            {
                _ = Append("(Unhandled parameter: ");
                _ = Append(parameterExpression.Name ?? "NoNameParameter");
                _ = Append(")");
            }
            else
            {
                _ = Append(parameterExpression.Name ?? "NoNameParameter");
            }
        }

        if(Verbose)
        {
            var parameterIndex = _encounteredParameters.Count;
            if(_encounteredParameters.Contains(parameterExpression))
            {
                parameterIndex = _encounteredParameters.IndexOf(parameterExpression);
            }
            else
            {
                _encounteredParameters.Add(parameterExpression);
            }

            _stringBuilder.Append("{" + parameterIndex + "}");
        }

        return parameterExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitUnary(UnaryExpression unaryExpression)
    {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch(unaryExpression.NodeType)
        {
            case ExpressionType.Convert:
                _stringBuilder.Append("(" + unaryExpression.Type.Name + ")");

                if(unaryExpression.Operand is BinaryExpression)
                {
                    _stringBuilder.Append("(");
                    _ = Visit(unaryExpression.Operand);
                    _stringBuilder.Append(")");
                }
                else
                {
                    _ = Visit(unaryExpression.Operand);
                }

                break;

            case ExpressionType.Throw:
                _stringBuilder.Append("throw ");
                _ = Visit(unaryExpression.Operand);
                break;

            case ExpressionType.Not:
                _stringBuilder.Append("!(");
                _ = Visit(unaryExpression.Operand);
                _stringBuilder.Append(")");
                break;

            case ExpressionType.TypeAs:
                _stringBuilder.Append("(");
                _ = Visit(unaryExpression.Operand);
                _stringBuilder.Append(" as " + unaryExpression.Type.Name + ")");
                break;

            case ExpressionType.Quote:
                _ = Visit(unaryExpression.Operand);
                break;

            default:
                UnhandledExpressionType(unaryExpression);
                break;
        }

        return unaryExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitDefault(DefaultExpression defaultExpression)
    {
        _stringBuilder.Append("default(" + defaultExpression.Type.Name + ")");

        return defaultExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitTry(TryExpression tryExpression)
    {
        _stringBuilder.Append("try { ");
        _ = Visit(tryExpression.Body);
        _stringBuilder.Append(" } ");

        foreach(var handler in tryExpression.Handlers)
        {
            _stringBuilder.Append("catch (" + handler.Test.Name + ") { ... } ");
        }

        return tryExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitIndex(IndexExpression indexExpression)
    {
        _ = Visit(indexExpression.Object);
        _stringBuilder.Append("[");
        VisitArguments(
            indexExpression.Arguments, s => _stringBuilder.Append(s));
        _stringBuilder.Append("]");

        return indexExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitTypeBinary(TypeBinaryExpression typeBinaryExpression)
    {
        _stringBuilder.Append("(");
        _ = Visit(typeBinaryExpression.Expression);
        _stringBuilder.Append(" is " + typeBinaryExpression.TypeOperand.Name + ")");

        return typeBinaryExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitSwitch(SwitchExpression switchExpression)
    {
        _stringBuilder.Append("switch (");
        _ = Visit(switchExpression.SwitchValue);
        _stringBuilder.AppendLine(")");
        _stringBuilder.AppendLine("{");
        _ = _stringBuilder.Indent();

        foreach(var @case in switchExpression.Cases)
        {
            foreach(var testValue in @case.TestValues)
            {
                _stringBuilder.Append("case ");
                _ = Visit(testValue);
                _stringBuilder.AppendLine(": ");
            }

            using(_stringBuilder.Indent())
            {
                _ = Visit(@case.Body);
            }

            _stringBuilder.AppendLine();
        }

        if(switchExpression.DefaultBody != null)
        {
            _stringBuilder.AppendLine("default: ");
            using(_stringBuilder.Indent())
            {
                _ = Visit(switchExpression.DefaultBody);
            }

            _stringBuilder.AppendLine();
        }

        _ = _stringBuilder.DeIndent();
        _stringBuilder.AppendLine("}");

        return switchExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitInvocation(InvocationExpression invocationExpression)
    {
        _stringBuilder.Append("Invoke(");
        _ = Visit(invocationExpression.Expression);

        foreach(var argument in invocationExpression.Arguments)
        {
            _stringBuilder.Append(", ");
            _ = Visit(argument);
        }

        _stringBuilder.Append(")");

        return invocationExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitExtension(Expression extensionExpression)
    {
        UnhandledExpressionType(extensionExpression);

        return extensionExpression;
    }

    /// <summary>
    /// Visits the arguments.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="lastSeparator">The last separator.</param>
    /// <param name="areConnected">If true, are connected.</param>
    private void VisitArguments(
        IReadOnlyList<Expression> arguments,
        Action<string> appendAction,
        string lastSeparator = "",
        bool areConnected = false)
    {
        for(var i = 0; i < arguments.Count; i++)
        {
            if(areConnected && i == arguments.Count - 1)
            {
                _ = Append("");
            }

            _ = Visit(arguments[i]);
            appendAction(i == arguments.Count - 1 ? lastSeparator : ", ");
        }
    }

    /// <summary>
    /// Posts the process.
    /// </summary>
    /// <param name="printedExpression">The printed expression.</param>
    /// <returns>A string.</returns>
    private static string PostProcess(string printedExpression)
    {
        var processedPrintedExpression = printedExpression
            .Replace("Microsoft.EntityFrameworkCore.Query.", "")
            .Replace("Microsoft.EntityFrameworkCore.", "")
            .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

        return processedPrintedExpression;
    }

    /// <summary>
    /// Unhandleds the expression type.
    /// </summary>
    /// <param name="expression">The expression.</param>
    private void UnhandledExpressionType(Expression expression)
    {
        _ = AppendLine(expression.ToString());
    }
}
