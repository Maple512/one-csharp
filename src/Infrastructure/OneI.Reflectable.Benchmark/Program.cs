using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using OneI.Reflectable;

var test = new ReflectionType();

var config = ManualConfig.CreateEmpty()
    .AddLogger(ConsoleLogger.Unicode)
    .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(1, exportGithubMarkdown: false)))
    .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)))
    .AddDiagnoser(new NativeMemoryProfiler())
    .AddDiagnoser(ThreadingDiagnoser.Default)
    .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared))
    .AddColumn(new RankColumn(NumeralSystem.Arabic))
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddExporter(MarkdownExporter.GitHub);

BenchmarkRunner.Run<OneI.Reflectable.ReflectionType>(config);
