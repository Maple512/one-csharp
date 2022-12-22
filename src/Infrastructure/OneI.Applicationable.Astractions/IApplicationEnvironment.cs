namespace OneI.Applicationable;
/// <summary>
/// The application environment.
/// </summary>

public interface IApplicationEnvironment
{
    /// <summary>
    /// Gets the environment name.
    /// </summary>
    string? EnvironmentName { get; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// Gets the root path.
    /// </summary>
    string RootPath { get; }
}
