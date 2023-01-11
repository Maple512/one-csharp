namespace OneI.Utilityable.Benchmark;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkTool.RunAssymbly<Program>(args);

#if DEBUG
        var validators = typeof(Program).Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IValidator)) && x.IsClass)
            .Select(x =>
            {
                return (IValidator)Activator.CreateInstance(x)!;
            })
            .ToArray();

        foreach(var item in validators)
        {
            item.Validate();
        }
#endif
        BenchmarkTool.Run<IntParseBenchmark>(args);
    }
}
