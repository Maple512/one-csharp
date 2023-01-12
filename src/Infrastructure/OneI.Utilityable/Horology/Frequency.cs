namespace OneI.Horology;

using System.Diagnostics.Contracts;
using System.Globalization;

// source from: https://github.com/AndreyAkinshin/perfolizer
public readonly struct Frequency : IEquatable<Frequency>, IComparable<Frequency>
{
    private const string DefaultFormat = null!;

    public double Hertz { get; }

    public Frequency(double hertz) => Hertz = hertz;

    public Frequency(double value, FrequencyUnit unit) : this(value * unit.HertzAmount) { }

    public static readonly Frequency Zero = new(0);
    public static readonly Frequency Hz = FrequencyUnit.Hz.ToFrequency();
    public static readonly Frequency KHz = FrequencyUnit.KHz.ToFrequency();
    public static readonly Frequency MHz = FrequencyUnit.MHz.ToFrequency();
    public static readonly Frequency GHz = FrequencyUnit.GHz.ToFrequency();

    [Pure] public TimeInterval ToResolution() => TimeInterval.Second / Hertz;

    [Pure] public double ToHz() => this / Hz;
    [Pure] public double ToKHz() => this / KHz;
    [Pure] public double ToMHz() => this / MHz;
    [Pure] public double ToGHz() => this / GHz;

    [Pure] public static Frequency FromHz(double value) => Hz * value;
    [Pure] public static Frequency FromKHz(double value) => KHz * value;
    [Pure] public static Frequency FromMHz(double value) => MHz * value;
    [Pure] public static Frequency FromGHz(double value) => GHz * value;

    [Pure]
    public static implicit operator Frequency(double value)
    {
        return new(value);
    }

    [Pure]
    public static implicit operator double(Frequency property)
    {
        return property.Hertz;
    }

    [Pure]
    public static double operator /(Frequency a, Frequency b)
    {
        return 1.0 * a.Hertz / b.Hertz;
    }

    [Pure]
    public static Frequency operator /(Frequency a, double k)
    {
        return new(a.Hertz / k);
    }

    [Pure]
    public static Frequency operator /(Frequency a, int k)
    {
        return new(a.Hertz / k);
    }

    [Pure]
    public static Frequency operator *(Frequency a, double k)
    {
        return new(a.Hertz * k);
    }

    [Pure]
    public static Frequency operator *(Frequency a, int k)
    {
        return new(a.Hertz * k);
    }

    [Pure]
    public static Frequency operator *(double k, Frequency a)
    {
        return new(a.Hertz * k);
    }

    [Pure]
    public static Frequency operator *(int k, Frequency a)
    {
        return new(a.Hertz * k);
    }

    [Pure]
    public static bool operator <(Frequency a, Frequency b)
    {
        return a.Hertz < b.Hertz;
    }

    [Pure]
    public static bool operator >(Frequency a, Frequency b)
    {
        return a.Hertz > b.Hertz;
    }

    [Pure]
    public static bool operator <=(Frequency a, Frequency b)
    {
        return a.Hertz <= b.Hertz;
    }

    [Pure]
    public static bool operator >=(Frequency a, Frequency b)
    {
        return a.Hertz >= b.Hertz;
    }

    [Pure]
    public static bool operator ==(Frequency a, Frequency b)
    {
        return a.Hertz.Equals(b.Hertz);
    }

    [Pure]
    public static bool operator !=(Frequency a, Frequency b)
    {
        return !a.Hertz.Equals(b.Hertz);
    }

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, out Frequency freq)
    {
        return TryParse(s, unit, NumberStyles.Any, CultureInfo.InvariantCulture, out freq);
    }

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, FrequencyUnit unit, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
    {
        var success = double.TryParse(s, numberStyle, formatProvider, out var result);
        freq = new Frequency(result, unit);
        return success;
    }

    [Pure] public static bool TryParseHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.Hz, out freq);
    [Pure]
    public static bool TryParseHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.Hz, numberStyle, formatProvider, out freq);

    [Pure] public static bool TryParseKHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.KHz, out freq);
    [Pure]
    public static bool TryParseKHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.KHz, numberStyle, formatProvider, out freq);

    [Pure] public static bool TryParseMHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.MHz, out freq);
    [Pure]
    public static bool TryParseMHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.MHz, numberStyle, formatProvider, out freq);

    [Pure] public static bool TryParseGHz([NotNullWhen(true)] string? s, out Frequency freq) => TryParse(s, FrequencyUnit.GHz, out freq);
    [Pure]
    public static bool TryParseGHz([NotNullWhen(true)] string? s, NumberStyles numberStyle, IFormatProvider formatProvider, out Frequency freq)
        => TryParse(s, FrequencyUnit.GHz, numberStyle, formatProvider, out freq);

    [Pure]
    public string ToString(
        string? format,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        return ToString(null, format, formatProvider, unitPresentation);
    }

    [Pure]
    public string ToString(
        FrequencyUnit? frequencyUnit,
        string? format = null,
        IFormatProvider? formatProvider = null,
        UnitPresentation? unitPresentation = null)
    {
        frequencyUnit ??= FrequencyUnit.GetBestFrequencyUnit(Hertz);
        format ??= DefaultFormat;
        formatProvider ??= CultureInfo.InvariantCulture;
        unitPresentation ??= UnitPresentation.Default;
        var unitValue = FrequencyUnit.Convert(Hertz, FrequencyUnit.Hz, frequencyUnit);
        if(unitPresentation.Value.IsVisible)
        {
            var unitName = frequencyUnit.Value.Name.PadLeft(unitPresentation.Value.MinUnitWidth);
            return $"{unitValue.ToString(format, formatProvider)} {unitName}";
        }

        return unitValue.ToString(format, formatProvider);
    }

    [Pure] public override string ToString() => ToString(DefaultFormat);

    public bool Equals(Frequency other) => Hertz.Equals(other.Hertz);
    public bool Equals(Frequency other, double hertzEpsilon) => Math.Abs(Hertz - other.Hertz) < hertzEpsilon;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Frequency other && Equals(other);
    public override int GetHashCode() => Hertz.GetHashCode();
    public int CompareTo(Frequency other) => Hertz.CompareTo(other.Hertz);
}
