namespace OneT.Benchmark;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

public static class BenchmarkTool
{
    private static readonly IConfig config = ManualConfig.CreateEmpty()
        .WithSummaryStyle(SummaryStyle.Default)
        .AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray())
        .AddValidator(DefaultConfig.Instance.GetValidators().ToArray())
        .AddLogger(ConsoleLogger.Unicode)
        .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(1, exportGithubMarkdown: false)))
        .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
        .AddDiagnoser(ThreadingDiagnoser.Default)
        .AddDiagnoser(new NativeMemoryProfiler())
        .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared))
        .AddColumn(new RankColumn(NumeralSystem.Arabic))
        .AddColumnProvider(DefaultColumnProviders.Instance)
        .AddJob(Job.Default.AsBaseline().AsDefault().WithArguments(new[] { new MsBuildArgument("/p:Optimize=true /p:AllowUnsafeBlocks=true") }))
        .AddExporter(MarkdownExporter.GitHub, HtmlExporter.Default)
        .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DontOverwriteResults);

    public static void RunAssymbly<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        configure?.Invoke(config);

        BenchmarkRunner.Run(typeof(T).Assembly, config, args);
    }

    public static void Run<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        configure?.Invoke(config);

        BenchmarkRunner.Run(typeof(T), config, args);
    }
}
