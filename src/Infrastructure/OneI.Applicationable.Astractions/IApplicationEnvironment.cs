namespace OneI.Applicationable;

public interface IApplicationEnvironment
{
    string? EnvironmentName { get; }

    string ApplicationName { get; }

    string RootPath { get; }
}
