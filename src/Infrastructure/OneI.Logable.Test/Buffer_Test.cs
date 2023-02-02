namespace OneI.Logable;

public class Buffer_Test
{
    [Fact]
    public async Task buffer()
        => await Parallel.ForEachAsync(Enumerable.Range(0, 100), async (_, _) =>
        {
            await Run();
        });

    private Task Run()
    {
        var result = A();

        if(result is not "a")
        {
            throw new Exception("asdfasdfasdf");
        }

        return Task.CompletedTask;
    }

    private string A()
    {
        scoped Span<char> buffer = stackalloc char[1];

        buffer.Fill('a');

        ref var first = ref buffer[0];

        return char.ToString(first);
    }
}
