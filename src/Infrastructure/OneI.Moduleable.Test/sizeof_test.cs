namespace OneI.Moduleable;

public class sizeof_test
{
    [Fact]
    public void print_size()
    {
        TestTools.PrintLayoutToFile<Type>();

        TestTools.PrintLayoutToFile<RuntimeTypeHandle>();
    }
}
