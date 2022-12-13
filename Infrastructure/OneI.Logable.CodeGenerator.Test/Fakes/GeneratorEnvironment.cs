namespace OneI.Logable.Fakes;

using System;

/// <summary>
/// The user type.
/// </summary>
public enum UserType { a, b, c, d }

/// <summary>
/// The user info.
/// </summary>
[Serializable]
public class UserInfo
{
    /// <summary>
    /// Gets or Sets the id.
    /// </summary>
    /// <value>An int.</value>
    public int Id { get; set; }
}

/// <summary>
/// The model1.
/// </summary>
public class Model1
{
    /// <summary>
    /// Gets or Sets the name.
    /// </summary>
    /// <value>An int.</value>
    public int Name { get; set; }
}
