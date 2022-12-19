``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.2364/21H2/November2021Update)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|      Method |     Mean |    Error |   StdDev | Ratio | Rank |   Gen0 | Completed Work Items | Lock Contentions | Code Size | Allocated native memory | Native memory leak | Allocated | Alloc Ratio |
|------------ |---------:|---------:|---------:|------:|-----:|-------:|---------------------:|-----------------:|----------:|------------------------:|-------------------:|----------:|------------:|
| UseProperty | 59.09 ns | 0.266 ns | 0.249 ns |  1.00 |    3 | 0.0076 |                    - |                - |      74 B |                       - |                  - |      48 B |        1.00 |
|       UseIL | 15.91 ns | 0.339 ns | 0.441 ns |  0.27 |    2 | 0.0038 |                    - |                - |      74 B |                       - |                  - |      24 B |        0.50 |
|   UseLambda | 15.33 ns | 0.206 ns | 0.192 ns |  0.26 |    1 | 0.0038 |                    - |                - |      74 B |                       - |                  - |      24 B |        0.50 |
