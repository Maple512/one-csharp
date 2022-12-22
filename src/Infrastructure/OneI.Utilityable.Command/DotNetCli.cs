namespace OneI;
/// <summary>
/// The dot net cli.
/// </summary>

public static class DotNetCli
{
    private static string? _binPath;

    /// <summary>
    /// Gets the bin path.
    /// </summary>
    public static string BinPath
    {
        get
        {
            if(_binPath == null)
            {
                var corelibPath = typeof(object).Assembly.Location;
                _binPath = Directory.GetParent(corelibPath)!.Parent!.Parent!.Parent!.FullName;
            }

            return _binPath!;
        }
    }

    /// <summary>
    /// Gets the dotnet executable path.
    /// </summary>
    public static string DotnetExecutablePath => Path.Combine(BinPath, RuntimeInformationHelper.GetExeFileName("dotnet"));

    /// <summary>
    /// Execs the.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Exec(string command, params string[] args)
    {
        var newArgs = args.ToList();
        newArgs.Insert(0, command);

        // https://learn.microsoft.com/zh-cn/dotnet/core/tools/dotnet-environment-variables
        return Command.Create(DotnetExecutablePath, newArgs)
            .EnvironmentVariable("DOTNET_SKIP_FIRST_TIME_EXPERIENCE", "1"); // Avoid looking at machine state by default
    }

    /// <summary>
    /// Restores the.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Restore(params string[] args)
    {
        return Exec("restore", args);
    }

    /// <summary>
    /// Builds the.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Build(params string[] args)
    {
        return Exec("build", args);
    }

    /// <summary>
    /// Tests the.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Test(params string[] args)
    {
        return Exec("test", args);
    }

    /// <summary>
    /// Cleans the.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>A Command.</returns>
    public static Command Clean(params string[] args)
    {
        return Exec("clean", args);
    }
}
