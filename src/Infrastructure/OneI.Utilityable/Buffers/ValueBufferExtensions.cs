namespace OneI.Buffers;

using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using OneI.Collections.Generic;
using static OneI.Buffers.ValueHelper;

/// <summary>
/// source: <see href="https://github.com/dotnet/runtime/blob/ac2ffdf4ff87e7e3a5506a8ef69ce633f6fcda85/src/libraries/System.Private.CoreLib/src/System/MemoryExtensions.cs"/>
/// </summary>
[StackTraceHidden]
public static partial class ValueBufferExtensions
{
    /// <summary>
    /// Searches for the specified value and returns true if found. If not found, returns false. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool Contains<T>(this ValueBuffer<T> span, T value)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.Contains(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified value and returns true if found. If not found, returns false. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool Contains<T>(this ReadOnlyValueBuffer<T> span, T value)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(Unsafe.AsRef(span.GetReference()))),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.ContainsValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.Contains(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified value and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ValueBuffer<T> span, T value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.IndexOfValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.IndexOfValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.IndexOf(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified sequence and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The sequence to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> value)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOf(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        if(sizeof(T) == sizeof(char))
        {
            return ValueHelper.IndexOf(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        return ValueHelper.IndexOf(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(value.GetReference()), value.Length);
    }

    /// <summary>
    /// Searches for the specified value and returns the index of its last occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ValueBuffer<T> span, T value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.LastIndexOf<T>(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified sequence and returns the index of its last occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The sequence to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOf(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }
        else if(sizeof(T) == sizeof(char))
        {
            return ValueHelper.LastIndexOf(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        return ValueHelper.LastIndexOf<T>(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(value.GetReference()), value.Length);
    }

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">A value to avoid.</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value"/>.
    /// If all of the values are <paramref name="value"/>, returns -1.
    /// </returns>
    public static int IndexOfAnyExcept<T>(this ValueBuffer<T> span, T value)
        where T : unmanaged, IEquatable<T>?
        => IndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value);

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value0"/> or <paramref name="value1"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value0"/> and <paramref name="value1"/>.
    /// If all of the values are <paramref name="value0"/> or <paramref name="value1"/>, returns -1.
    /// </returns>
    public static int IndexOfAnyExcept<T>(this ValueBuffer<T> span, T value0, T value1)
        where T : unmanaged, IEquatable<T>? =>
        IndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value0, value1);

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value0"/>, <paramref name="value1"/>, or <paramref name="value2"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <param name="value2">A value to avoid</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>.
    /// If all of the values are <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>, returns -1.
    /// </returns>
    public static int IndexOfAnyExcept<T>(this ValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>? =>
        IndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value0, value1, value2);

    /// <summary>Searches for the first index of any value other than the specified <paramref name="values"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The values to avoid.</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than those in <paramref name="values"/>.
    /// If all of the values are in <paramref name="values"/>, returns -1.
    /// </returns>
    public static int IndexOfAnyExcept<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> values) where T : unmanaged, IEquatable<T>? =>
        IndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, values);

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">A value to avoid.</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value"/>.
    /// If all of the values are <paramref name="value"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            Debug.Assert(sizeof(T) == sizeof(long));

            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.IndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value0"/> or <paramref name="value1"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value0"/> and <paramref name="value1"/>.
    /// If all of the values are <paramref name="value0"/> or <paramref name="value1"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.IndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>Searches for the first index of any value other than the specified <paramref name="value0"/>, <paramref name="value1"/>, or <paramref name="value2"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <param name="value2">A value to avoid</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>.
    /// If all of the values are <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.IndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int IndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2, T value3) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                Unsafe.As<T, byte>(ref value3),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                Unsafe.As<T, short>(ref value3),
                span.Length);
        }

        return ValueHelper.IndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, value3, span.Length);
    }

    /// <summary>Searches for the first index of any value other than the specified <paramref name="values"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The values to avoid.</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than those in <paramref name="values"/>.
    /// If all of the values are in <paramref name="values"/>, returns -1.
    /// </returns>
    public static unsafe int IndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> values) where T : unmanaged, IEquatable<T>?
    {
        switch(values.Length)
        {
            case 0:
                // If the span is empty, we want to return -1.
                // If the span is non-empty, we want to return the index of the first char that's not in the empty set,
                // which is every character, and so the first char in the span.
                return span.IsEmpty ? -1 : 0;
            case 1:
                return IndexOfAnyExcept(span, values[0]);
            case 2:
                return IndexOfAnyExcept(span, values[0], values[1]);
            case 3:
                return IndexOfAnyExcept(span, values[0], values[1], values[2]);
            case 4:
                return IndexOfAnyExcept(span, values[0], values[1], values[2], values[3]);
            default:
                for(var i = 0; i < span.Length; i++)
                {
                    if(!values.Contains(span[i]))
                    {
                        return i;
                    }
                }

                return -1;
        }
    }

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">A value to avoid.</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value"/>.
    /// If all of the values are <paramref name="value"/>, returns -1.
    /// </returns>
    public static int LastIndexOfAnyExcept<T>(this ValueBuffer<T> span, T value) where T : unmanaged, IEquatable<T>? =>
        LastIndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value);

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value0"/> or <paramref name="value1"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value0"/> and <paramref name="value1"/>.
    /// If all of the values are <paramref name="value0"/> or <paramref name="value1"/>, returns -1.
    /// </returns>
    public static int LastIndexOfAnyExcept<T>(this ValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>? =>
        LastIndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value0, value1);

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value0"/>, <paramref name="value1"/>, or <paramref name="value2"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <param name="value2">A value to avoid</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>.
    /// If all of the values are <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>, returns -1.
    /// </returns>
    public static int LastIndexOfAnyExcept<T>(this ValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>? =>
        LastIndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, value0, value1, value2);

    /// <summary>Searches for the last index of any value other than the specified <paramref name="values"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The values to avoid.</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than those in <paramref name="values"/>.
    /// If all of the values are in <paramref name="values"/>, returns -1.
    /// </returns>
    public static int LastIndexOfAnyExcept<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> values) where T : unmanaged, IEquatable<T>? =>
        LastIndexOfAnyExcept((ReadOnlyValueBuffer<T>)span, values);

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">A value to avoid.</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value"/>.
    /// If all of the values are <paramref name="value"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            Debug.Assert(sizeof(T) == sizeof(long));

            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.LastIndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value0"/> or <paramref name="value1"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value0"/> and <paramref name="value1"/>.
    /// If all of the values are <paramref name="value0"/> or <paramref name="value1"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.LastIndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>Searches for the last index of any value other than the specified <paramref name="value0"/>, <paramref name="value1"/>, or <paramref name="value2"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">A value to avoid.</param>
    /// <param name="value1">A value to avoid</param>
    /// <param name="value2">A value to avoid</param>
    /// <returns>
    /// The index in the span of the last occurrence of any value other than <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>.
    /// If all of the values are <paramref name="value0"/>, <paramref name="value1"/>, and <paramref name="value2"/>, returns -1.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.LastIndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int LastIndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2, T value3)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                Unsafe.As<T, byte>(ref value3),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyExceptValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                Unsafe.As<T, short>(ref value3),
                span.Length);
        }

        return ValueHelper.LastIndexOfAnyExcept(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, value3, span.Length);
    }

    /// <summary>Searches for the last index of any value other than the specified <paramref name="values"/>.</summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The values to avoid.</param>
    /// <returns>
    /// The index in the span of the first occurrence of any value other than those in <paramref name="values"/>.
    /// If all of the values are in <paramref name="values"/>, returns -1.
    /// </returns>
    public static unsafe int LastIndexOfAnyExcept<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> values)
        where T : unmanaged, IEquatable<T>?
    {
        switch(values.Length)
        {
            case 0:
                // If the span is empty, we want to return -1.
                // If the span is non-empty, we want to return the index of the last char that's not in the empty set,
                // which is every character, and so the last char in the span.
                // Either way, we want to return span.Length - 1.
                return span.Length - 1;

            case 1:
                return LastIndexOfAnyExcept(span, values[0]);

            case 2:
                return LastIndexOfAnyExcept(span, values[0], values[1]);

            case 3:
                return LastIndexOfAnyExcept(span, values[0], values[1], values[2]);

            case 4:
                return LastIndexOfAnyExcept(span, values[0], values[1], values[2], values[3]);

            default:
                for(var i = span.Length - 1; i >= 0; i--)
                {
                    if(!values.Contains(span[i]))
                    {
                        return i;
                    }
                }

                return -1;
        }
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements using IEquatable{T}.Equals(T).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool SequenceEqual<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged, IEquatable<T>?
    {
        var length = span.Length;

        var size = (nuint)sizeof(T);

        return length == other.Length &&
        ValueHelper.SequenceEqual(
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
            (uint)length * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this api in such a case so we choose not to take the overhead of checking.
    }

    /// <summary>
    /// Determines the relative order of the sequences being compared by comparing the elements using IComparable{T}.CompareTo(T).
    /// </summary>
    public static int SequenceCompareTo<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
       where T : unmanaged, IComparable<T>?
    {
        // Can't use IsBitwiseEquatable<T>() below because that only tells us about
        // equality checks, not about CompareTo checks.

        if(typeof(T) == typeof(byte))
        {
            return ValueHelper.SequenceCompareTo(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
                other.Length);
        }

        if(typeof(T) == typeof(char))
        {
            return ValueHelper.SequenceCompareTo(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(other.GetReference())),
                other.Length);
        }

        return ValueHelper.SequenceCompareTo(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(other.GetReference()), other.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfCore<T>(this ReadOnlyValueBuffer<T> span, T value)
        where T : unmanaged,IEqualityOperators<T,T,bool>
    {
            if(Unsafe.SizeOf<T>() == 1)
            {
                return ValueHelper.IndexOfValueTypeaaa<byte, DontNegate<byte>>(
                    ref Unsafe.As<T, byte>(ref span.GetReference()),
                    Unsafe.As<T, byte>(ref value),
                    span.Length);
            }
            if(Unsafe.SizeOf<T>() == 2)
            {
                return IndexOfValueTypeaaa<short, DontNegate<short>>(
                    ref Unsafe.As<T, short>(ref span.GetReference()),
                    Unsafe.As<T, short>(ref value), span.Length);
            }
            if(Unsafe.SizeOf<T>() == 4)
            {
                return IndexOfValueTypeaaa<int, DontNegate<int>>(
                    ref Unsafe.As<T, int>(ref span.GetReference()),
                    Unsafe.As<T, int>(ref value), span.Length);
            }
            if(Unsafe.SizeOf<T>() == 8)
            {
                return IndexOfValueTypeaaa<long, DontNegate<long>>(
                    ref Unsafe.As<T, long>(ref span.GetReference()),
                    Unsafe.As<T, long>(ref value), span.Length);
            }

        return IndexOaaaf(ref span.GetReference(), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified value and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlyValueBuffer<T> span, T value)
        where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfValueTypeCore(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfValueTypeCore(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.IndexOfValueTypeCore(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }

        if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.IndexOfValueTypeCore(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.IndexOf(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified sequence and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The sequence to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOf(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        if(sizeof(T) == sizeof(char))
        {
            return ValueHelper.IndexOf(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        return ValueHelper.IndexOf(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(value.GetReference()), value.Length);
    }

    /// <summary>
    /// Searches for the specified value and returns the index of its last occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ReadOnlyValueBuffer<T> span, T value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, int>(ref value),
                span.Length);
        }
        else if(sizeof(T) == sizeof(long))
        {
            return ValueHelper.LastIndexOfValueType(
                ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, long>(ref value),
                span.Length);
        }

        return ValueHelper.LastIndexOf<T>(ref Unsafe.AsRef(span.GetReference()), value, span.Length);
    }

    /// <summary>
    /// Searches for the specified sequence and returns the index of its last occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T).
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The sequence to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOf<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> value) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOf(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }
        if(sizeof(T) == sizeof(char))
        {
            return ValueHelper.LastIndexOf(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(value.GetReference())),
                value.Length);
        }

        return ValueHelper.LastIndexOf<T>(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(value.GetReference()), value.Length);
    }

    /// <summary>
    /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.IndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>
    /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <param name="value2">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.IndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    /// <summary>
    /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.IndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>
    /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <param name="value2">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOfAny<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.IndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.IndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.LastIndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <param name="value2">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.LastIndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOfAny<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> values)
        where T : unmanaged, IEquatable<T>?
        => LastIndexOfAny((ReadOnlyValueBuffer<T>)span, values);

    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> values)
        where T : unmanaged, IEquatable<T>?
    {
        ref var r1 = ref Unsafe.AsRef(span.GetReference());
        ref var r2 = ref Unsafe.AsRef(values.GetReference());
        if(sizeof(T) == sizeof(byte))
        {
            ref byte spanRef = ref Unsafe.As<T, byte>(ref r1);
            ref byte valueRef = ref Unsafe.As<T, byte>(ref r2);
            switch(values.Length)
            {
                case 0:
                    return -1;

                case 1:
                    return ValueHelper.LastIndexOfValueType(ref spanRef, valueRef, span.Length);

                case 2:
                    return ValueHelper.LastIndexOfAnyValueType(
                        ref spanRef,
                        valueRef,
                        Unsafe.Add(ref valueRef, 1),
                        span.Length);

                case 3:
                    return ValueHelper.LastIndexOfAnyValueType(
                        ref spanRef,
                        valueRef,
                        Unsafe.Add(ref valueRef, 1),
                        Unsafe.Add(ref valueRef, 2),
                        span.Length);

                case 4:
                    return ValueHelper.LastIndexOfAnyValueType(
                        ref spanRef,
                        valueRef,
                        Unsafe.Add(ref valueRef, 1),
                        Unsafe.Add(ref valueRef, 2),
                        Unsafe.Add(ref valueRef, 3),
                        span.Length);

                case 5:
                    return ValueHelper.LastIndexOfAnyValueType(
                        ref spanRef,
                        valueRef,
                        Unsafe.Add(ref valueRef, 1),
                        Unsafe.Add(ref valueRef, 2),
                        Unsafe.Add(ref valueRef, 3),
                        Unsafe.Add(ref valueRef, 4),
                        span.Length);
            }
        }

        if(sizeof(T) == sizeof(short))
        {
            ref short spanRef = ref Unsafe.As<T, short>(ref r1);
            ref short valueRef = ref Unsafe.As<T, short>(ref r2);
            return values.Length switch
            {
                0 => -1,
                1 => ValueHelper.LastIndexOfValueType(ref spanRef, valueRef, span.Length),
                2 => ValueHelper.LastIndexOfAnyValueType(
                                        ref spanRef,
                                        valueRef,
                                        Unsafe.Add(ref valueRef, 1),
                                        span.Length),
                3 => ValueHelper.LastIndexOfAnyValueType(
                                        ref spanRef,
                                        valueRef,
                                        Unsafe.Add(ref valueRef, 1),
                                        Unsafe.Add(ref valueRef, 2),
                                        span.Length),
                4 => ValueHelper.LastIndexOfAnyValueType(
                                        ref spanRef,
                                        valueRef,
                                        Unsafe.Add(ref valueRef, 1),
                                        Unsafe.Add(ref valueRef, 2),
                                        Unsafe.Add(ref valueRef, 3),
                                        span.Length),
                5 => ValueHelper.LastIndexOfAnyValueType(
                                        ref spanRef,
                                        valueRef,
                                        Unsafe.Add(ref valueRef, 1),
                                        Unsafe.Add(ref valueRef, 2),
                                        Unsafe.Add(ref valueRef, 3),
                                        Unsafe.Add(ref valueRef, 4),
                                        span.Length),
                _ => ProbabilisticMap.LastIndexOfAny(ref Unsafe.As<short, char>(ref spanRef), span.Length, ref Unsafe.As<short, char>(ref valueRef), values.Length),
            };
        }

        return ValueHelper.LastIndexOfAny(ref r1, span.Length, ref r2, values.Length);
    }


    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                span.Length);
        }

        return ValueHelper.LastIndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, span.Length);
    }

    /// <summary>
    /// Searches for the last index of any of the specified values similar to calling LastIndexOf several times with the logical OR operator. If not found, returns -1.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <param name="value2">One of the values to search for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int LastIndexOfAny<T>(this ReadOnlyValueBuffer<T> span, T value0, T value1, T value2) where T : unmanaged, IEquatable<T>?
    {
        if(sizeof(T) == sizeof(byte))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, byte>(ref value0),
                Unsafe.As<T, byte>(ref value1),
                Unsafe.As<T, byte>(ref value2),
                span.Length);
        }
        else if(sizeof(T) == sizeof(short))
        {
            return ValueHelper.LastIndexOfAnyValueType(
                ref Unsafe.As<T, short>(ref Unsafe.AsRef(span.GetReference())),
                Unsafe.As<T, short>(ref value0),
                Unsafe.As<T, short>(ref value1),
                Unsafe.As<T, short>(ref value2),
                span.Length);
        }

        return ValueHelper.LastIndexOfAny(ref Unsafe.AsRef(span.GetReference()), value0, value1, value2, span.Length);
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements using IEquatable{T}.Equals(T).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool SequenceEqual<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged, IEquatable<T>?
    {
        var length = span.Length;
        var size = (nuint)sizeof(T);
        return length == other.Length &&
            ValueHelper.SequenceEqual(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
                (uint)length * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this API in such a case so we choose not to take the overhead of checking.
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements using an <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or null to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns>true if the two sequences are equal; otherwise, false.</returns>
    public static bool SequenceEqual<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other, IEqualityComparer<T>? comparer = null)
        where T : unmanaged
        => SequenceEqual((ReadOnlyValueBuffer<T>)span, other, comparer);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements using an <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or null to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns>true if the two sequences are equal; otherwise, false.</returns>
    public static unsafe bool SequenceEqual<T>(
        this ReadOnlyValueBuffer<T> span,
        ReadOnlyValueBuffer<T> other,
        IEqualityComparer<T>? comparer = null)
        where T : unmanaged
    {
        // If the spans differ in length, they're not equal.
        if(span.Length != other.Length)
        {
            return false;
        }

        if(typeof(T).IsValueType)
        {
            if(comparer is null || comparer == EqualityComparer<T>.Default)
            {
                // If no comparer was supplied and the type is bitwise equatable, take the fast path doing a bitwise comparison.
                var size = (nuint)sizeof(T);
                return ValueHelper.SequenceEqual(
                    ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                    ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
                    (uint)span.Length * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this API in such a case so we choose not to take the overhead of checking.
            }
        }

        // Use the comparer to compare each element.
        comparer ??= EqualityComparer<T>.Default;
        for(var i = 0; i < span.Length; i++)
        {
            if(!comparer.Equals(span[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines the relative order of the sequences being compared by comparing the elements using IComparable{T}.CompareTo(T).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int SequenceCompareTo<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
       where T : unmanaged, IComparable<T>?
    {
        // Can't use IsBitwiseEquatable<T>() below because that only tells us about
        // equality checks, not about CompareTo checks.

        if(typeof(T) == typeof(byte))
        {
            return ValueHelper.SequenceCompareTo(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
                other.Length);
        }

        if(typeof(T) == typeof(char))
        {
            return ValueHelper.SequenceCompareTo(
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(span.GetReference())),
                span.Length,
                ref Unsafe.As<T, char>(ref Unsafe.AsRef(other.GetReference())),
                other.Length);
        }

        return ValueHelper.SequenceCompareTo(ref Unsafe.AsRef(span.GetReference()), span.Length, ref Unsafe.AsRef(other.GetReference()), other.Length);
    }

    /// <summary>
    /// Determines whether the specified sequence appears at the start of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool StartsWith<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> value) where T : unmanaged, IEquatable<T>?
    {
        var valueLength = value.Length;

        {
            var size = (nuint)sizeof(T);
            return valueLength <= span.Length &&
            ValueHelper.SequenceEqual(
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
                ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
                (uint)valueLength * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this api in such a case so we choose not to take the overhead of checking.
        }

        return valueLength <= span.Length && ValueHelper.SequenceEqual(ref Unsafe.AsRef(span.GetReference()), ref Unsafe.AsRef(value.GetReference()), valueLength);
    }

    /// <summary>
    /// Determines whether the specified sequence appears at the start of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool StartsWith<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> value)
        where T : unmanaged, IEquatable<T>?
    {
        var valueLength = value.Length;

        var size = (nuint)sizeof(T);

        return valueLength <= span.Length &&
        ValueHelper.SequenceEqual(
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
            (uint)valueLength * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this api in such a case so we choose not to take the overhead of checking.
    }

    /// <summary>
    /// Determines whether the specified sequence appears at the end of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool EndsWith<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> value) where T : unmanaged, IEquatable<T>?
    {
        var spanLength = span.Length;
        var valueLength = value.Length;

        var size = (nuint)sizeof(T);
        return valueLength <= spanLength &&
        ValueHelper.SequenceEqual(
            ref Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(span.GetReference()), (nint)(uint)(spanLength - valueLength) /* force zero-extension */)),
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
            (uint)valueLength * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this api in such a case so we choose not to take the overhead of checking.
    }

    /// <summary>
    /// Determines whether the specified sequence appears at the end of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool EndsWith<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> value)
        where T : unmanaged, IEquatable<T>?
    {
        var spanLength = span.Length;
        var valueLength = value.Length;

        var size = (nuint)sizeof(T);
        return valueLength <= spanLength &&
        ValueHelper.SequenceEqual(
            ref Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(span.GetReference()), (nint)(uint)(spanLength - valueLength) /* force zero-extension */)),
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(value.GetReference())),
            (uint)valueLength * size);  // If this multiplication overflows, the Span we got overflows the entire address range. There's no happy outcome for this api in such a case so we choose not to take the overhead of checking.
    }

    /// <summary>
    /// Reverses the sequence of the elements in the entire span.
    /// </summary>
    public static void Reverse<T>(this ValueBuffer<T> span)
        where T : unmanaged
    {
        if(span.Length > 1)
        {
            ValueHelper.Reverse(ref Unsafe.AsRef(span.GetReference()), (nuint)span.Length);
        }
    }

    //
    //  Overlaps
    //  ========
    //
    //  The following methods can be used to determine if two sequences
    //  overlap in memory.
    //
    //  Two sequences overlap if they have positions in common and neither
    //  is empty. Empty sequences do not overlap with any other sequence.
    //
    //  If two sequences overlap, the element offset is the number of
    //  elements by which the second sequence is offset from the first
    //  sequence (i.e., second minus first). An exception is thrown if the
    //  number is not a whole number, which can happen when a sequence of a
    //  smaller type is cast to a sequence of a larger type with unsafe code
    //  or NonPortableCast. If the sequences do not overlap, the offset is
    //  meaningless and arbitrarily set to zero.
    //
    //  Implementation
    //  --------------
    //
    //  Implementing this correctly is quite tricky due of two problems:
    //
    //  * If the sequences refer to two different objects on the managed
    //    heap, the garbage collector can move them freely around or change
    //    their relative order in memory.
    //
    //  * The distance between two sequences can be greater than
    //    int.MaxValue (on a 32-bit system) or long.MaxValue (on a 64-bit
    //    system).
    //
    //  (For simplicity, the following text assumes a 32-bit system, but
    //  everything also applies to a 64-bit system if every 32 is replaced a
    //  64.)
    //
    //  The first problem is solved by calculating the distance with exactly
    //  one atomic operation. If the garbage collector happens to move the
    //  sequences afterwards and the sequences overlapped before, they will
    //  still overlap after the move and their distance hasn't changed. If
    //  the sequences did not overlap, the distance can change but the
    //  sequences still won't overlap.
    //
    //  The second problem is solved by making all addresses relative to the
    //  start of the first sequence and performing all operations in
    //  unsigned integer arithmetic modulo 2^32.
    //
    //  Example
    //  -------
    //
    //  Let's say there are two sequences, x and y. Let
    //
    //      ref T xRef = MemoryMarshal.GetReference(x)
    //      uint xLength = x.Length * sizeof(T)
    //      ref T yRef = MemoryMarshal.GetReference(y)
    //      uint yLength = y.Length * sizeof(T)
    //
    //  Visually, the two sequences are located somewhere in the 32-bit
    //  address space as follows:
    //
    //      [----------------------------------------------)                            normal address space
    //      0                                             2^32
    //                            [------------------)                                  first sequence
    //                            xRef            xRef + xLength
    //              [--------------------------)     .                                  second sequence
    //              yRef          .         yRef + yLength
    //              :             .            .     .
    //              :             .            .     .
    //                            .            .     .
    //                            .            .     .
    //                            .            .     .
    //                            [----------------------------------------------)      relative address space
    //                            0            .     .                          2^32
    //                            [------------------)             :                    first sequence
    //                            x1           .     x2            :
    //                            -------------)                   [-------------       second sequence
    //                                         y2                  y1
    //
    //  The idea is to make all addresses relative to xRef: Let x1 be the
    //  start address of x in this relative address space, x2 the end
    //  address of x, y1 the start address of y, and y2 the end address of
    //  y:
    //
    //      nuint x1 = 0
    //      nuint x2 = xLength
    //      nuint y1 = (nuint)Unsafe.ByteOffset(xRef, yRef)
    //      nuint y2 = y1 + yLength
    //
    //  xRef relative to xRef is 0.
    //
    //  x2 is simply x1 + xLength. This cannot overflow.
    //
    //  yRef relative to xRef is (yRef - xRef). If (yRef - xRef) is
    //  negative, casting it to an unsigned 32-bit integer turns it into
    //  (yRef - xRef + 2^32). So, in the example above, y1 moves to the right
    //  of x2.
    //
    //  y2 is simply y1 + yLength. Note that this can overflow, as in the
    //  example above, which must be avoided.
    //
    //  The two sequences do *not* overlap if y is entirely in the space
    //  right of x in the relative address space. (It can't be left of it!)
    //
    //          (y1 >= x2) && (y2 <= 2^32)
    //
    //  Inversely, they do overlap if
    //
    //          (y1 < x2) || (y2 > 2^32)
    //
    //  After substituting x2 and y2 with their respective definition:
    //
    //      == (y1 < xLength) || (y1 + yLength > 2^32)
    //
    //  Since yLength can't be greater than the size of the address space,
    //  the overflow can be avoided as follows:
    //
    //      == (y1 < xLength) || (y1 > 2^32 - yLength)
    //
    //  However, 2^32 cannot be stored in an unsigned 32-bit integer, so one
    //  more change is needed to keep doing everything with unsigned 32-bit
    //  integers:
    //
    //      == (y1 < xLength) || (y1 > -yLength)
    //
    //  Due to modulo arithmetic, this gives exactly same result *except* if
    //  yLength is zero, since 2^32 - 0 is 0 and not 2^32. So the case
    //  y.IsEmpty must be handled separately first.
    //

    /// <summary>
    /// Determines whether two sequences overlap in memory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged
    {
        return Overlaps((ReadOnlyValueBuffer<T>)span, other);
    }

    /// <summary>
    /// Determines whether two sequences overlap in memory and outputs the element offset.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other, out int elementOffset)
        where T : unmanaged
    {
        return Overlaps((ReadOnlyValueBuffer<T>)span, other, out elementOffset);
    }

    /// <summary>
    /// Determines whether two sequences overlap in memory.
    /// </summary>
    public static unsafe bool Overlaps<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged
    {
        if(span.IsEmpty || other.IsEmpty)
        {
            return false;
        }

        var byteOffset = Unsafe.ByteOffset(
            ref Unsafe.AsRef(span.GetReference()),
            ref Unsafe.AsRef(other.GetReference()));

        return (nuint)byteOffset < (nuint)((nint)span.Length * sizeof(T)) ||
                (nuint)byteOffset > (nuint)(-((nint)other.Length * sizeof(T)));
    }

    /// <summary>
    /// Determines whether two sequences overlap in memory and outputs the element offset.
    /// </summary>
    public static unsafe bool Overlaps<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other, out int elementOffset)
        where T : unmanaged
    {
        if(span.IsEmpty || other.IsEmpty)
        {
            elementOffset = 0;
            return false;
        }

        var byteOffset = Unsafe.ByteOffset(
            ref Unsafe.AsRef(span.GetReference()),
            ref Unsafe.AsRef(other.GetReference()));

        if((nuint)byteOffset < (nuint)((nint)span.Length * sizeof(T)) ||
            (nuint)byteOffset > (nuint)(-((nint)other.Length * sizeof(T))))
        {
            if(byteOffset % sizeof(T) != 0)
            {
                ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();
            }

            elementOffset = (int)(byteOffset / sizeof(T));
            return true;
        }
        else
        {
            elementOffset = 0;
            return false;
        }
    }

    /// <summary>
    /// Searches an entire sorted <see cref="Span{T}"/> for a value
    /// using the specified <see cref="IComparable{T}"/> generic interface.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <param name="span">The sorted <see cref="Span{T}"/> to search.</param>
    /// <param name="comparable">The <see cref="IComparable{T}"/> to use when comparing.</param>
    /// <returns>
    /// The zero-based index of <paramref name="comparable"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="comparable"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="comparable"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="Span{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparable" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T>(this ValueBuffer<T> span, IComparable<T> comparable)
        where T : unmanaged
    {
        return BinarySearch<T, IComparable<T>>(span, comparable);
    }

    /// <summary>
    /// Searches an entire sorted <see cref="Span{T}"/> for a value
    /// using the specified <typeparamref name="TComparable"/> generic type.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <typeparam name="TComparable">The specific type of <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The sorted <see cref="Span{T}"/> to search.</param>
    /// <param name="comparable">The <typeparamref name="TComparable"/> to use when comparing.</param>
    /// <returns>
    /// The zero-based index of <paramref name="comparable"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="comparable"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="comparable"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="Span{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparable" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparable>(
        this ValueBuffer<T> span, TComparable comparable)
        where TComparable : IComparable<T>
        where T : unmanaged
    {
        return BinarySearch((ReadOnlyValueBuffer<T>)span, comparable);
    }

    /// <summary>
    /// Searches an entire sorted <see cref="Span{T}"/> for the specified <paramref name="value"/>
    /// using the specified <typeparamref name="TComparer"/> generic type.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <typeparam name="TComparer">The specific type of <see cref="IComparer{T}"/>.</typeparam>
    /// <param name="span">The sorted <see cref="Span{T}"/> to search.</param>
    /// <param name="value">The object to locate. The value can be null for reference types.</param>
    /// <param name="comparer">The <typeparamref name="TComparer"/> to use when comparing.</param>
    /// /// <returns>
    /// The zero-based index of <paramref name="value"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="value"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="value"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="Span{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparer" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparer>(this ValueBuffer<T> span, T value, TComparer comparer)
        where TComparer : IComparer<T>
        where T : unmanaged
    {
        return BinarySearch((ReadOnlyValueBuffer<T>)span, value, comparer);
    }

    /// <summary>
    /// Searches an entire sorted <see cref="ReadOnlyValueBuffer{T}"/> for a value
    /// using the specified <see cref="IComparable{T}"/> generic interface.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <param name="span">The sorted <see cref="ReadOnlyValueBuffer{T}"/> to search.</param>
    /// <param name="comparable">The <see cref="IComparable{T}"/> to use when comparing.</param>
    /// <returns>
    /// The zero-based index of <paramref name="comparable"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="comparable"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="comparable"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="ReadOnlyValueBuffer{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparable" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T>(this ReadOnlyValueBuffer<T> span, IComparable<T> comparable)
        where T : unmanaged
    {
        return BinarySearch<T, IComparable<T>>(span, comparable);
    }

    /// <summary>
    /// Searches an entire sorted <see cref="ReadOnlyValueBuffer{T}"/> for a value
    /// using the specified <typeparamref name="TComparable"/> generic type.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <typeparam name="TComparable">The specific type of <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The sorted <see cref="ReadOnlyValueBuffer{T}"/> to search.</param>
    /// <param name="comparable">The <typeparamref name="TComparable"/> to use when comparing.</param>
    /// <returns>
    /// The zero-based index of <paramref name="comparable"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="comparable"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="comparable"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="ReadOnlyValueBuffer{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparable" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparable>(this ReadOnlyValueBuffer<T> span, TComparable comparable)
        where T : unmanaged
        where TComparable : IComparable<T>
    {
        return ValueHelper.BinarySearch(span, comparable);
    }

    /// <summary>
    /// Searches an entire sorted <see cref="ReadOnlyValueBuffer{T}"/> for the specified <paramref name="value"/>
    /// using the specified <typeparamref name="TComparer"/> generic type.
    /// </summary>
    /// <typeparam name="T">The element type of the span.</typeparam>
    /// <typeparam name="TComparer">The specific type of <see cref="IComparer{T}"/>.</typeparam>
    /// <param name="span">The sorted <see cref="ReadOnlyValueBuffer{T}"/> to search.</param>
    /// <param name="value">The object to locate. The value can be null for reference types.</param>
    /// <param name="comparer">The <typeparamref name="TComparer"/> to use when comparing.</param>
    /// /// <returns>
    /// The zero-based index of <paramref name="value"/> in the sorted <paramref name="span"/>,
    /// if <paramref name="value"/> is found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than <paramref name="value"/> or, if there is
    /// no larger element, the bitwise complement of <see cref="ReadOnlyValueBuffer{T}.Length"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name = "comparer" /> is <see langword="null"/> .
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinarySearch<T, TComparer>(
        this ReadOnlyValueBuffer<T> span, T value, TComparer comparer)
        where TComparer : IComparer<T>
        where T : unmanaged
    {
        if(comparer == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparer);
        }

        var comparable = new ValueHelper.ComparerComparable<T, TComparer>(
            value, comparer);
        return BinarySearch(span, comparable);
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="Span{T}" /> using the <see cref="IComparable{T}" /> implementation
    /// of each element of the <see cref= "Span{T}" />
    /// </summary>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
    /// <exception cref="InvalidOperationException">
    /// One or more elements in <paramref name="span"/> do not implement the <see cref="IComparable{T}" /> interface.
    /// </exception>
    public static void Sort<T>(this ValueBuffer<T> span)
        where T : unmanaged
        => Sort(span, (IComparer<T>?)null);

    /// <summary>
    /// Sorts the elements in the entire <see cref="Span{T}" /> using the <typeparamref name="TComparer" />.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    /// <typeparam name="TComparer">The type of the comparer to use to compare elements.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing elements, or null to
    /// use the <see cref="IComparable{T}"/> interface implementation of each element.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="comparer"/> is null, and one or more elements in <paramref name="span"/> do not
    /// implement the <see cref="IComparable{T}" /> interface.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The implementation of <paramref name="comparer"/> caused an error during the sort.
    /// </exception>
    public static void Sort<T, TComparer>(this ValueBuffer<T> span, TComparer comparer)
        where T : unmanaged
        where TComparer : IComparer<T>?
    {
        if(span.Length > 1)
        {
            ArraySort<T>.Default.Sort(span, comparer); // value-type comparer will be boxed
        }
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="Span{T}" /> using the specified <see cref="Comparison{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to sort.</param>
    /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
    public static void Sort<T>(this ValueBuffer<T> span, Comparison<T> comparison)
        where T : unmanaged
    {
        if(comparison == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
        }

        if(span.Length > 1)
        {
            ArraySort<T>.Sort(span, comparison);
        }
    }

    /// <summary>
    /// Replaces all occurrences of <paramref name="oldValue"/> with <paramref name="newValue"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="span">The span in which the elements should be replaced.</param>
    /// <param name="oldValue">The value to be replaced with <paramref name="newValue"/>.</param>
    /// <param name="newValue">The value to replace all occurrences of <paramref name="oldValue"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Replace<T>(this ValueBuffer<T> span, T oldValue, T newValue)
        where T : unmanaged, IEquatable<T>?
    {
        nuint length = (uint)span.Length;

        if(sizeof(T) == sizeof(byte))
        {
            ref var src = ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference()));
            ValueHelper.ReplaceValueType(
                ref src,
                ref src,
                Unsafe.As<T, byte>(ref oldValue),
                Unsafe.As<T, byte>(ref newValue),
                length);
        }
        else if(sizeof(T) == sizeof(ushort))
        {
            // Use ushort rather than short, as this avoids a sign-extending move.
            ref var src = ref Unsafe.As<T, ushort>(ref Unsafe.AsRef(span.GetReference()));
            ValueHelper.ReplaceValueType(
                ref src,
                ref src,
                Unsafe.As<T, ushort>(ref oldValue),
                Unsafe.As<T, ushort>(ref newValue),
                length);
        }
        else if(sizeof(T) == sizeof(int))
        {
            ref var src = ref Unsafe.As<T, int>(ref Unsafe.AsRef(span.GetReference()));
            ValueHelper.ReplaceValueType(
                ref src,
                ref src,
                Unsafe.As<T, int>(ref oldValue),
                Unsafe.As<T, int>(ref newValue),
                length);
        }
        else
        {
            Debug.Assert(sizeof(T) == sizeof(long));

            ref var src = ref Unsafe.As<T, long>(ref Unsafe.AsRef(span.GetReference()));
            ValueHelper.ReplaceValueType(
                ref src,
                ref src,
                Unsafe.As<T, long>(ref oldValue),
                Unsafe.As<T, long>(ref newValue),
                length);
        }

        ValueHelper.Replace(span, oldValue, newValue);
    }

    /// <summary>Finds the length of any common prefix shared between <paramref name="span"/> and <paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the elements in the spans.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <returns>The length of the common prefix shared by the two spans.  If there's no shared prefix, 0 is returned.</returns>
    public static int CommonPrefixLength<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged
        => CommonPrefixLength((ReadOnlyValueBuffer<T>)span, other);

    /// <summary>Finds the length of any common prefix shared between <paramref name="span"/> and <paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the elements in the spans.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or null to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns>The length of the common prefix shared by the two spans.  If there's no shared prefix, 0 is returned.</returns>
    public static int CommonPrefixLength<T>(this ValueBuffer<T> span, ReadOnlyValueBuffer<T> other, IEqualityComparer<T>? comparer)
        where T : unmanaged
        => CommonPrefixLength((ReadOnlyValueBuffer<T>)span, other, comparer);

    /// <summary>Finds the length of any common prefix shared between <paramref name="span"/> and <paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the elements in the spans.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <returns>The length of the common prefix shared by the two spans.  If there's no shared prefix, 0 is returned.</returns>
    public static unsafe int CommonPrefixLength<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other)
        where T : unmanaged
    {
        var length = Math.Min((uint)span.Length, (nuint)(uint)other.Length);
        nuint size = (uint)sizeof(T);
        var index = ValueHelper.CommonPrefixLength(
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(span.GetReference())),
            ref Unsafe.As<T, byte>(ref Unsafe.AsRef(other.GetReference())),
            length * size);

        // A byte-wise comparison in CommonPrefixLength can be used for multi-byte types,
        // that are bitwise-equatable, too. In order to get the correct index in terms of type T
        // of the first mismatch, integer division by the size of T is used.
        //
        // Example for short:
        // index (byte-based):   b-1,  b,    b+1,    b+2,  b+3
        // index (short-based):  s-1,  s,            s+1
        // byte sequence 1:    { ..., [0x42, 0x43], [0x37, 0x38], ... }
        // byte sequence 2:    { ..., [0x42, 0x43], [0x37, 0xAB], ... }
        // So the mismatch is a byte-index b+3, which gives integer divided by the size of short:
        // 3 / 2 = 1, thus the expected index short-based.
        return (int)(index / size);
    }

    /// <summary>Determines the length of any common prefix shared between <paramref name="span"/> and <paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the elements in the sequences.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or null to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.</param>
    /// <returns>The length of the common prefix shared by the two spans.  If there's no shared prefix, 0 is returned.</returns>
    public static int CommonPrefixLength<T>(this ReadOnlyValueBuffer<T> span, ReadOnlyValueBuffer<T> other, IEqualityComparer<T>? comparer)
        where T : unmanaged
    {
        // If the comparer is null or the default, and T is a value type, we want to use EqualityComparer<T>.Default.Equals
        // directly to enable devirtualization.  The non-comparer overload already does so, so just use it.
        if(typeof(T).IsValueType && (comparer is null || comparer == EqualityComparer<T>.Default))
        {
            return CommonPrefixLength(span, other);
        }

        // Shrink one of the spans if necessary to ensure they're both the same length. We can then iterate until
        // the Length of one of them and at least have bounds checks removed from that one.
        SliceLongerSpanToMatchShorterLength(ref span, ref other);

        // Ensure we have a comparer, then compare the spans.
        comparer ??= EqualityComparer<T>.Default;
        for(var i = 0; i < span.Length; i++)
        {
            if(!comparer.Equals(span[i], other[i]))
            {
                return i;
            }
        }

        return span.Length;
    }

    /// <summary>Determines if one span is longer than the other, and slices the longer one to match the length of the shorter.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SliceLongerSpanToMatchShorterLength<T>(ref ReadOnlyValueBuffer<T> span, ref ReadOnlyValueBuffer<T> other)
        where T : unmanaged
    {
        if(other.Length > span.Length)
        {
            other = other[..span.Length];
        }
        else if(span.Length > other.Length)
        {
            span = span[..other.Length];
        }
        Debug.Assert(span.Length == other.Length);
    }

    internal static void CheckStringSplitOptions(StringSplitOptions options)
    {
        const StringSplitOptions AllValidFlags = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        if((options & ~AllValidFlags) != 0)
        {
            // at least one invalid flag was set
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidFlag, ExceptionArgument.options);
        }
    }
}
