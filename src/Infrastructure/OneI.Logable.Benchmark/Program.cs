using OneI.Logable;

Console.WriteLine();

var bench = new TemplateParserBenchmark();

bench.UseLog4net();

BenchmarkTool.Run<TemplateParserBenchmark>(args);
