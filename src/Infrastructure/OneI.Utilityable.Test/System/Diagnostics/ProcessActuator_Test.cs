namespace System.Diagnostics;

public class ProcessActuator_Test
{
    [Fact]
    public async Task Run_dotnet_info()
    {
        var parameters = new ProcessParameter("dotnet", "--info");

        var result = await ProcessActuator.RunAsync(parameters);

        result.ExitedCode.ShouldBe(0);
    }

    [Fact]
    public async Task Use_Cmd_Run_Command()
    {
        var result = await ProcessActuator.RunAsync(
            ProcessParameter.UseCMD("dotnet", "--info"));

        result.ExitedCode.ShouldBe(0);
    }
}
