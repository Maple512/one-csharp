namespace OneI;

public static class RuntimeInformationHelper
{
    public static string GetExeFileName(string exeName)
    {
        return $"{exeName}{(OperatingSystem.IsWindows() ? ".exe" : string.Empty)}";
    }
}
