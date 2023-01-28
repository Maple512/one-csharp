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
        .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(exportGithubMarkdown: false)))
        .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
        .AddDiagnoser(ThreadingDiagnoser.Default)
        .AddDiagnoser(new NativeMemoryProfiler())
        .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest))
        .AddColumn(new RankColumn(NumeralSystem.Arabic), CategoriesColumn.Default)
        .AddColumnProvider(DefaultColumnProviders.Instance)
        .AddJob(Job.Default.AsBaseline().AsDefault().WithArguments(new[] { new MsBuildArgument("/p:Optimize=true /p:AllowUnsafeBlocks=true") }))
        .AddExporter(MarkdownExporter.GitHub, HtmlExporter.Default, RPlotExporter.Default)
        .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DontOverwriteResults );

    public static void RunAssymbly<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        configure?.Invoke(config);

        BenchmarkRunner.Run(typeof(T).Assembly, config, args);
    }

    public static void Run<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        var startType = typeof(T);

        var validators = startType.Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IValidator)) && x.IsClass)
            .Select(x =>
            {
                return (IValidator)Activator.CreateInstance(x)!;
            })
            .ToArray();

        foreach(var item in validators)
        {
            item.Initialize();

            item.Validate();
        }

        configure?.Invoke(config);

        BenchmarkRunner.Run(startType, config, args);
    }
}
