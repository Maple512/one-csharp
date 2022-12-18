namespace OneI.Reflectable.SCG.ConsoleTest;

using System;

internal class Program
{
    static void Main(string[] args)
    {
        var type = args.GetOneType();

        Console.WriteLine("Hello, World!");
    }
}
