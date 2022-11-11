# 代码生成器案例

## 第一部分：创建一个增量生成器

我首先提供了关于源生成器的少量背景知识，以及在枚举上调用ToString（）的问题。在本文的剩余部分，我将逐步创建增量生成器。最后的结果是一个工作的源代码生成器，尽管有一些限制，正如我在文章末尾所描述的。

1. 创建源生成器项目
2. 收集有关枚举的详细信息
3. 添加标记属性
4. 创建增量源生成器
5. 构建增量生成器管道
6. 实施管道阶段
7. 分析EnumDeclarationSyntax以创建EnumToGenerate
8. 生成源代码
9. 局限性

### 背景：源生成器

源代码生成器是作为.NET5中的一个内置功能添加的。它们在编译时执行代码生成，从而能够自动将源代码添加到项目中。
这打开了一个广阔的可能性领域，但使用源生成器来替换原本需要使用反射来完成的事情的能力是公司最喜欢的。

我已经写了很多关于源代码生成器的帖子，例如：

- [使用源生成器查找Blazor WebAssembly应用程序中的所有可路由组件](https://andrewlock.net/using-source-generators-to-find-all-routable-components-in-a-webassembly-app/)
- [使用源生成器提高日志记录性能](https://andrewlock.net/exploring-dotnet-6-part-8-improving-logging-performance-with-source-generators/)
- [源生成器更新：增量生成器](https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/)

在.NET6中，引入了一个新的API来创建“增量生成器”。它们具有与.NET5中的源代码生成器大致相同的功能，但它们旨在利用缓存显著提高性能，这样IDE就不会慢下来！增量生成器的主要缺点是它们仅在.NET 6 SDK中受支持（因此仅在VS 2022中受支持）。

### 目的：enum和ToString()

c#中的简单枚举是表示选项选择的一个方便的小想法。在引擎盖下，它由一个数值（通常是int）表示，但不必在代码中记住0表示“红色”，1表示“蓝色”，您可以使用一个为您保存该信息的枚举：

```C#
public enum Colour // Yes, I'm British
{
    Red = 0,
    Blue = 1,
}
```

在您的代码中，您传递枚举Colour的实例，但在幕后，运行时实际上只使用int。问题是，有时您想要获得颜色的名称。内置的方法是调用ToString()

```c#
public void PrintColour(Colour colour)
{
    Console.Writeline("You chose "+ colour.ToString()); // You chose Red
}
```

阅读这篇文章的人可能都知道这一点。但这可能是一个不太常见的知识，这就是sloooow。我们将很快看到速度有多慢，但首先我们将看到使用现代C#的快速实现：

```C#
public static class EnumExtensions
{
    public string ToStringFast(this Colour colour)
        => colour switch
        {
            Colour.Red => nameof(Colour.Red),
            Colour.Blue => nameof(Colour.Blue),
            _ => colour.ToString(),
        }
    }
}
```

这个简单的switch语句检查Colour的每个已知值，并使用nameof返回枚举的文本表示。如果它是一个未知值，那么底层值将作为字符串返回。

您必须始终小心这些未知值：例如，这是有效的C# `PrintColour((Colour)123)`

如果我们将这个简单的switch语句与使用已知颜色的BenchmarkDotNet的默认ToString()实现进行比较，您可以看到我们的实现速度有多快：

Method       | FX	Mean | Error      | StdDev    | Ratio     | Gen 0 | Allocated |
------------ | ------- | ---------- | --------- | --------- | ----- | --------- |
EnumToString | net48   | 578.276 ns | 3.3109 ns | 3.0970 ns | 1.000 | 0.0458    | 96 | B |
ToStringFast | net48   | 3.091 ns   | 0.0567 ns | 0.0443 ns | 0.005 | -         | -  |
EnumToString | net6.0  | 17.9850 ns | 0.1230 ns | 0.1151 ns | 1.000 | 0.0115    | 24 | B |
ToStringFast | net6.0  | 0.1212 ns  | 0.0225 ns | 0.0199 ns | 0.007 | -         | -  |

首先，值得指出的是，.NET6中的ToString()比.NETFramework中的方法快30倍以上，只分配四分之一的字节！尽管与“Fast”版本相比，它还是超级慢！

尽管创建ToStringFast()方法的速度很快，但这有点困难，因为您必须确保在枚举更改时保持它最新。幸运的是，这是源代码生成器的完美用例！

