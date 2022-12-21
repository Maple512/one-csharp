namespace OneI;

public static class DotNetCli
{
    private static string? _binPath;

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

    public static string DotnetExecutablePath => Path.Combine(BinPath, RuntimeInformationHelper.GetExeFileName("dotnet"));

    public static Command Exec(string command, params string[] args)
    {
        var newArgs = args.ToList();
        newArgs.Insert(0, command);

        // https://learn.microsoft.com/zh-cn/dotnet/core/tools/dotnet-environment-variables
        return Command.Create(DotnetExecutablePath, newArgs)
            .EnvironmentVariable("DOTNET_SKIP_FIRST_TIME_EXPERIENCE", "1"); // Avoid looking at machine state by default
    }

    public static Command Restore(params string[] args) => Exec("restore", args);
    public static Command Build(params string[] args) => Exec("build", args);
    public static Command Test(params string[] args) => Exec("test", args);
    public static Command Clean(params string[] args) => Exec("clean", args);
}
