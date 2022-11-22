namespace OneI.Logable.Configurations;

using System;
using OneI.Logable.Properties.Policies;

public class LoggerDestructuringConfiguration
{
    private readonly LoggerConfiguration _loggerConfiguration;
    private readonly Action<Type> _scalarTypeAction;
    private readonly Action<IDestructuringPolicy> _destructurePolicyAction;
    private readonly Action<int> _maximumDepth;
    private readonly Action<int> _maximumStringLength;
    private readonly Action<int> _maximumCollectionCount;

    internal LoggerDestructuringConfiguration(
        LoggerConfiguration loggerConfiguration,
        Action<Type> scalarTypeAction,
        Action<IDestructuringPolicy> destructurePolicyAction,
        Action<int> maximumDepth,
        Action<int> maximumStringLength,
        Action<int> maximumCollectionCount)
    {
        _loggerConfiguration = loggerConfiguration;
        _scalarTypeAction = scalarTypeAction;
        _destructurePolicyAction = destructurePolicyAction;
        _maximumDepth = maximumDepth;
        _maximumStringLength = maximumStringLength;
        _maximumCollectionCount = maximumCollectionCount;
    }

    public LoggerConfiguration AsScalar(Type scalarType)
    {
        _scalarTypeAction(scalarType);

        return _loggerConfiguration;
    }

    public LoggerConfiguration AsScalar<TScalar>() => AsScalar(typeof(TScalar));

    public LoggerConfiguration With(params IDestructuringPolicy[] destructuringPolicies)
    {
        foreach(var destructuringPolicy in destructuringPolicies)
        {
            if(destructuringPolicy == null)
            {
                throw new ArgumentException("Null policy is not allowed.");
            }

            _destructurePolicyAction(destructuringPolicy);
        }

        return _loggerConfiguration;
    }

    public LoggerConfiguration With<TDestructuringPolicy>()
        where TDestructuringPolicy : IDestructuringPolicy, new()
    {
        return With(new TDestructuringPolicy());
    }

    public LoggerConfiguration ByTransforming<TValue>(Func<TValue, object> transformation)
    {
        var policy = new CustomDestructuringPolicy(
            t => t == typeof(TValue),
            o => transformation((TValue)o));

        return With(policy);
    }

    public LoggerConfiguration ByTransformingWhere<TValue>(
        Func<Type, bool> predicate,
        Func<TValue, object> transformation)
    {
        var policy = new CustomDestructuringPolicy(
            predicate,
            o => transformation((TValue)o));

        return With(policy);
    }

    public LoggerConfiguration MaximumDepth(int maximumDestructuringDepth)
    {
        if(maximumDestructuringDepth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumDestructuringDepth), "Maximum destructuring depth must be positive.");
        }

        _maximumDepth(maximumDestructuringDepth);

        return _loggerConfiguration;
    }

    public LoggerConfiguration MaximumStringLength(int maximumStringLength)
    {
        if(maximumStringLength < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumStringLength), maximumStringLength, "Maximum string length must be at least two.");
        }

        _maximumStringLength(maximumStringLength);

        return _loggerConfiguration;
    }

    public LoggerConfiguration MaximumCollectionCount(int maximumCollectionCount)
    {
        if(maximumCollectionCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumCollectionCount), maximumCollectionCount, "Maximum collection length must be at least one.");
        }

        _maximumCollectionCount(maximumCollectionCount);

        return _loggerConfiguration;
    }
}
