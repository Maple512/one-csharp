namespace OneI.Logable.Fakes;

[Serializable]
public class User
{
    public int Id { get; set; }

    public int Age { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public User1? Child { get; set; }
}

[Serializable]
public class User1
{
    public int Id { get; set; }

    public User2? Child { get; set; }
    public User2? Child1 { get; set; }
    public User2? Child2 { get; set; }
}

public class User2
{
    public int Id { get; set; }

    public User3? Child { get; set; }
    public User3? Child1 { get; set; }
    public User3? Child2 { get; set; }
    public User3? Child3 { get; set; }
    public User3? Child4 { get; set; }
}

[Serializable]
public class User3
{
    public int Id { get; set; }

    public User4? Child { get; set; }
}

[Serializable]
public class User4 : ICustomFormatter
{
    public int Id { get; set; }

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }
}

public class ModelFormattable : IFormattable
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"{nameof(ModelFormattable)} {DateTime.Now:mm:ss}";
    }
}
