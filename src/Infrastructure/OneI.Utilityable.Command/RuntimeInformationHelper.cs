namespace OneI;
/// <summary>
/// The runtime information helper.
/// </summary>

public static class RuntimeInformationHelper
{
    /// <summary>
    /// Gets the exe file name.
    /// </summary>
    /// <param name="exeName">The exe name.</param>
    /// <returns>A string.</returns>
    public static string GetExeFileName(string exeName)
    {
        return $"{exeName}{(OperatingSystem.IsWindows() ? ".exe" : string.Empty)}";
    }
}
