namespace OneI.Logable.Middlewares;

using System.Runtime.InteropServices;
using OneI.Logable.Diagnostics;
using OneI.Logable.Fakes;

public class Environment_Test
{
    [Fact]
    public void environment_values()
    {
        var logger = Fake.CreateLogger(template: "{ProcessId}, {FrameworkDescription}, {CommandLine}", configuration: c =>
        {
            c.WithEnvironment(new EnvironmentOptions
            {
                HasProcessId = true,
                HasFrameworkDescription = true,
                HasCommandLine = true,
            });
            ;
        });

        logger.Information("information");

        TestAuditSink.Properties.ContainsKey(nameof(Environment.ProcessId)).ShouldBeTrue();
        TestAuditSink.Properties.ContainsKey(nameof(Environment.CommandLine)).ShouldBeTrue();
        TestAuditSink.Properties.ContainsKey(nameof(RuntimeInformation.FrameworkDescription)).ShouldBeTrue();
    }
}
