using BenchmarkDotNet.Configs;
using OneI.Logable;

BenchmarkTool.Run<TemplateParserBenchmark>(args, config =>
{
    config.WithOptions(ConfigOptions.DisableOptimizationsValidator);
});

Console.ReadLine();
