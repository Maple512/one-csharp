namespace OneT.Benchmark;

using System.Reflection;
using BenchmarkDotNet.Attributes;
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
using OneI;

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
        .AddExporter(MarkdownExporter.GitHub, HtmlExporter.Default)
        .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DontOverwriteResults | ConfigOptions.KeepBenchmarkFiles);

    public static void RunAssymbly<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        configure?.Invoke(config);

        var assembly = typeof(T).Assembly;

        Inlitialize(assembly);

        var unused = BenchmarkRunner.Run(assembly, config, args);
    }

    public static void Run<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        var startType = typeof(T);

        Inlitialize(startType.Assembly);

        configure?.Invoke(config);

        var unused = BenchmarkRunner.Run(startType, config, args);
    }

    public static void Switcher<T>(string[]? args = null, Action<IConfig>? configure = null)
    {
        configure?.Invoke(config);

        var startType = typeof(T);

        var assembly = startType.Assembly;

        Inlitialize(assembly);

        var unused = BenchmarkSwitcher.FromAssembly(assembly).Run(args, config);
    }

    [Conditional(SharedConstants.DEBUG)]
    private static void Inlitialize(Assembly assembly)
    {
        var benchmarks = assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(BenchmarkItem)) && x.IsClass && !x.IsAbstract)
            .Select(x => (BenchmarkItem)Activator.CreateInstance(x)!)
            .ToArray();

        foreach(var item in benchmarks)
        {
            var unused1 = TryToSetParamsFields(item);
            var unused = TryToSetParamsProperties(item);

            item.GlobalInlitialize();
        }
    }

    private static bool TryToSetParamsFields(object instance)
    {
        var paramFields = instance
                .GetType()
                .GetAllFields()
                .Where(fieldInfo => fieldInfo.GetCustomAttributes(false).OfType<ParamsAttribute>().Any())
                .ToArray();

        if(!paramFields.Any())
        {
            return true;
        }

        foreach(var paramField in paramFields)
        {
            if(!paramField.IsPublic)
            {
                continue;
            }

            var values = paramField.GetCustomAttributes(false).OfType<ParamsAttribute>().Single().Values;
            if(!values.Any())
            {
                continue;
            }

            try
            {
                paramField.SetValue(instance, values.First());
            }
            catch(Exception ex)
            {
                ex.ReThrow();
            }
        }

        return true;
    }

    private static bool TryToSetParamsProperties(object instance)
    {
        var paramProperties = instance
                .GetType()
                .GetAllProperties()
                .Where(propertyInfo => propertyInfo.GetCustomAttributes(false).OfType<ParamsAttribute>().Any())
                .ToArray();

        if(!paramProperties.Any())
        {
            return true;
        }

        foreach(var paramProperty in paramProperties)
        {
            var setter = paramProperty.SetMethod;
            if(setter == null || !setter.IsPublic)
            {
                continue;
            }

            var values = paramProperty.GetCustomAttributes(false).OfType<ParamsAttribute>().Single().Values;
            if(!values.Any())
            {
                continue;
            }

            try
            {
                var unused = setter.Invoke(instance, new[] { values.First() });
            }
            catch(Exception ex)
            {
                ex.ReThrow();
            }
        }

        return true;
    }
}
