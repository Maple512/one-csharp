namespace System.IO.Pipelines;

internal sealed class DuplexPipe : IDuplexPipe
{
    public DuplexPipe(PipeReader input, PipeWriter output)
    {
        Input = input;
        Output = output;
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }

    public static DuplexPipePair CreateConnection(PipeOptions inputOptions, PipeOptions outputOptions)
    {
        var input = new Pipe(inputOptions);
        var output = new Pipe(outputOptions);

        var server = new DuplexPipe(input.Reader, output.Writer);

        var client = new DuplexPipe(output.Reader, input.Writer);

        return new DuplexPipePair(server, client);
    }

    /// <summary>
    /// 双工管道对（包括：Server和Client）
    /// </summary>
    public readonly struct DuplexPipePair
    {
        public DuplexPipePair(IDuplexPipe server, IDuplexPipe client)
        {
            ServerPipe = server;
            ClientPipe = client;
        }

        public IDuplexPipe ServerPipe { get; }

        public IDuplexPipe ClientPipe { get; }
    }
}
