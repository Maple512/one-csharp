namespace OneI.Logable;

using System;
using System.Collections.Generic;

public static class TestHelpler
{
    public static IEnumerable<User> CreateNewUser()
    {
        yield return new User()
        {
            Id = 1,
            Name = "Maple512",
            Description = "just a mad man",
        };
    }
}

[Serializable]
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
