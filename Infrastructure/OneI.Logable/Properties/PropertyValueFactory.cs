namespace OneI.Logable.Properties;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using OneI.Logable.Properties.Policies;
using OneI.Logable.Properties.PropertyValues;

public class PropertyValueFactory : IPropertyValueFactory, IPropertyFactory
{
    /// <summary>
    /// 是否忽略异常，并将异常信息转化为字符串输出到日志中
    /// </summary>
    private readonly bool _propagateException;

    /// <summary>
    /// 允许的最大集合的长度
    /// </summary>
    private readonly int _maximumCollectionLength;

    private readonly IEnumerable<IScalarConversionPolicy> _scalarConversions;

    private readonly IEnumerable<IDestructuringPolicy> _destructuringPolicies;

    private readonly DepthLimiter _depthLimiter;

    /// <param name="propagateException">是否允许刨抛出异常</param>
    /// <param name="maximumCollectionLength">处理集合的最大长度</param>
    /// <param name="maxByteArrayLength">处理byte数组的最大长度</param>
    /// <param name="scalarConversionTypes"></param>
    /// <param name="destructuringPolicies"></param>
    /// <param name="maximumDeconstructDepth">最大解构深度</param>
    public PropertyValueFactory(
        IEnumerable<Type> scalarConversionTypes,
        IEnumerable<IDestructuringPolicy> destructuringPolicies,
        bool propagateException,
        int maximumCollectionLength,
        int maxByteArrayLength,
        int maximumDeconstructDepth)
    {
        _propagateException = propagateException;
        _maximumCollectionLength = maximumCollectionLength;

        _scalarConversions = new IScalarConversionPolicy[]
        {
            new CurrencyScalarConversionPolicy(scalarConversionTypes),
            new ByteArrayScalarConversionPolicy(maxByteArrayLength),
            new EnumScalarConversionPolicy(),
        };
        _destructuringPolicies = destructuringPolicies;

        _depthLimiter = new(this, maximumDeconstructDepth);
    }

    public Property Create(string name, object? value, bool deconstruct = false)
    {
        return new Property(name, Create(value, deconstruct));
    }

    /// <param name="deconstruct">是否将<paramref name="value"/>解构，类似将对象序列化</param>
    public PropertyValue Create(object? value, bool deconstruct = false)
    {
        return CreatePropertyValue(value, deconstruct);
    }

    private PropertyValue CreatePropertyValue(object? value, bool deconstruct = false)
    {
        if(value is null or string)
        {
            return new ScalarPropertyValue(value?.ToString() ?? null);
        }

        foreach(var item in _scalarConversions)
        {
            if(item.TryConvert(value, out var scalarPropertyValue))
            {

                return scalarPropertyValue;
            }
        }

        _depthLimiter.Increment();

        PropertyValue? propertyValue;
        if(deconstruct)
        {
            foreach(var item in _destructuringPolicies)
            {
                if(item.TryDestructure(value, _depthLimiter, out propertyValue))
                {
                    return propertyValue;
                }
            }
        }

        if(TryProcessEnumerable(value, deconstruct, out propertyValue))
        {
            return propertyValue;
        }

        if(TryProcessValueTulpe(value, deconstruct, out propertyValue))
        {
            return propertyValue;
        }

        if(TryConvertAnonymousType(value, deconstruct, out propertyValue))
        {
            return propertyValue;
        }

        return new ScalarPropertyValue(value);
    }

    private bool TryProcessEnumerable(object value, bool deconstruct, [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;
        var type = value.GetType();

        if(value is IEnumerable enumerable)
        {
            if(enumerable is IDictionary dictionary)
            {
                propertyValue = new DicationaryPropertyValue(MapToDicationaryElements(dictionary, deconstruct).ToArray());
            }
            else
            {
                propertyValue = new SequencePropertyValue(MapToSequenElements(enumerable, deconstruct).ToArray());
            }
        }

        return propertyValue != null;
    }

    private IEnumerable<KeyValuePair<ScalarPropertyValue, PropertyValue>> MapToDicationaryElements(IDictionary dictionary, bool deconstruct)
    {
        var count = 0;

        foreach(DictionaryEntry item in dictionary)
        {
            if(++count > _maximumCollectionLength)
            {
                yield break;
            }

            yield return new KeyValuePair<ScalarPropertyValue, PropertyValue>(
                (ScalarPropertyValue)_depthLimiter.Create(item.Key, deconstruct),
                _depthLimiter.Create(item.Value, deconstruct));
        }
    }

    private IEnumerable<PropertyValue> MapToSequenElements(IEnumerable enumerable, bool deconstruct)
    {
        var count = 0;

        foreach(var item in enumerable)
        {
            if(++count > _maximumCollectionLength)
            {
                yield break;
            }

            yield return _depthLimiter.Create(item, deconstruct);
        }
    }

    private bool TryProcessValueTulpe(object value, bool deconstruct, [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;

        if(value is ITuple tuple)
        {
            var values = new List<PropertyValue>(tuple.Length);
            for(var i = 0; i < tuple.Length; i++)
            {
                var item = tuple[i];

                values.Add(_depthLimiter.Create(value, deconstruct));
            }

            propertyValue = new SequencePropertyValue(values);
        }

        return propertyValue != null;
    }

    private bool TryConvertAnonymousType(object value, bool deconstruct, [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;

        if(deconstruct)
        {
            var type = value.GetType();

            var properties = GetAnonymousTypeProperties(value);

            propertyValue = new StructurePropertyValue(properties, type.IsAnonymousType() ? null : type.Name);
        }

        return propertyValue != null;
    }

    /// <summary>
    /// 获取匿名类型的属性
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private IEnumerable<Property> GetAnonymousTypeProperties(object value)
    {
        var properties = GetRecursiveProperties(value.GetType());

        foreach(var property in properties)
        {
            PropertyValue? propertyValue = null;
            try
            {
                var objectValue = property.GetValue(value);

                propertyValue = _depthLimiter.Create(objectValue, true);
            }
            catch(TargetParameterCountException)
            {
                continue;
            }
            catch(TargetInvocationException ex)
            {
                if(_propagateException)
                {
                    throw;
                }

                propertyValue = new ScalarPropertyValue($"The property accessor {property.Name} threw exception: {ex}.");
            }
            catch(NotSupportedException)
            {
                if(_propagateException)
                {
                    throw;
                }

                propertyValue = new ScalarPropertyValue($"The property accessor {property.Name} is not supported via Reflection API.");
            }

            yield return new Property(property.Name, propertyValue);
        }
    }

    private static IEnumerable<PropertyInfo> GetRecursiveProperties(Type type)
    {
        var names = new HashSet<string>();

        var currentType = type.GetTypeInfo();

        while(currentType.AsType() != typeof(object))
        {
            var properties = currentType.DeclaredProperties
                .Where(x => x.CanRead && x.GetMethod!.IsPublic
                && !x.GetMethod!.IsStatic
                && x.GetIndexParameters().Length == 0);

            foreach(var item in properties)
            {
                if(names.Add(item.Name))
                {
                    yield return item;
                }
            }

            if(currentType.BaseType == null)
            {
                yield break;
            }

            currentType = currentType.BaseType.GetTypeInfo();
        }
    }

    private class DepthLimiter : IPropertyValueFactory
    {
        private readonly AsyncLocal<int> _currentDepth = new();
        private readonly PropertyValueFactory _valueFactory;
        private readonly int _maximumDestructuringDepth;

        public DepthLimiter(PropertyValueFactory valueFactory, int maximumDestructuringDepth)
        {
            _valueFactory = valueFactory;
            _maximumDestructuringDepth = maximumDestructuringDepth;
        }

        internal void Increment() => _currentDepth.Value++;

        public PropertyValue Create(object? value, bool deconstruct = false)
        {
            var depth = _currentDepth.Value;

            if(depth == _maximumDestructuringDepth)
            {
                return new ScalarPropertyValue(null);
            }

            var result = _valueFactory.CreatePropertyValue(value, deconstruct);

            return result;
        }
    }
}
