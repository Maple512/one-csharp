namespace OneI.Openable.Connections.Sockets;

using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using OneI.Logable;
using OneI.Openable.Connections.Sockets.Internal;

public sealed class SocketConnectionContextFactory : IDisposable
{
    private SocketConnectionFactoryOptions _options;
    private ILogger _logger;
    private int _settingsCount;
    private long _index;
    private readonly QueueSetting[] _settings;

    public SocketConnectionContextFactory(
        SocketConnectionFactoryOptions options, ILogger logger)
    {
        _options = options;
        _logger = logger;
        _settingsCount = _options.IOQueueCount;

        var readBufferMaxLength = _options.ReadBufferMaxLength ?? 0L;
        var writeBufferMaxLength = _options.WriteBufferMaxLength ?? 0L;
        var serverScheduler = options.UnsafePreferInlineScheduler
            ? PipeScheduler.Inline
            : PipeScheduler.ThreadPool;

        if(_settingsCount > 0)
        {
            _settings = new QueueSetting[_settingsCount];
            for(var i = 0; i < _settingsCount; i++)
            {
                var memoryPool = new PinnedBlockMemoryPool();
                var clientScheduler = options.UnsafePreferInlineScheduler
                    ? PipeScheduler.Inline
                    : new IOQueue();

                _settings[i] = new()
                {
                    Scheduler = clientScheduler,
                    InputOptions = new(memoryPool, serverScheduler, clientScheduler, readBufferMaxLength, readBufferMaxLength / 2, -1, false),
                    OutputOptions = new(memoryPool, clientScheduler, serverScheduler, writeBufferMaxLength, writeBufferMaxLength / 2, -1, false),
                    SenderPool = new(PipeScheduler.Inline),
                    MemoryPool = memoryPool,
                };
            }
        }
        else
        {
            var memoryPool = new PinnedBlockMemoryPool();
            var clientScheduler = options.UnsafePreferInlineScheduler
                ? PipeScheduler.Inline
                : PipeScheduler.ThreadPool;

            _settings = new QueueSetting[]
            {
                new()
                {
                    Scheduler = clientScheduler,
                    InputOptions = new(memoryPool, serverScheduler, clientScheduler, readBufferMaxLength, readBufferMaxLength / 2, -1, false),
                    OutputOptions = new(memoryPool, clientScheduler, serverScheduler, writeBufferMaxLength, writeBufferMaxLength / 2, -1, false),
                    SenderPool = new(PipeScheduler.Inline),
                    MemoryPool = memoryPool,
                }
            };
            _settingsCount = 1;
        }
    }

    public ConnectionContext Create(Socket socket)
    {
        var setting = _settings[Interlocked.Increment(ref _index) % _settingsCount];

        var connection = new SocketConnection(
            socket,
            setting.MemoryPool,
            setting.SenderPool.Scheduler,
            _logger,
            setting.SenderPool,
            setting.InputOptions,
            setting.OutputOptions,
            _options.WaitForDataBeforeAllocatingBuffer);

        connection.Start();

        return connection;
    }

    public void Dispose()
    {
        for(var i = 0; i < _settingsCount; i++)
        {
            var settings = _settings[i];
            settings.SenderPool.Dispose();
            settings.MemoryPool.Dispose();
        }
    }

    private sealed class QueueSetting
    {
        public PipeScheduler Scheduler { get; init; } = default!;

        public PipeOptions InputOptions { get; init; } = default!;

        public PipeOptions OutputOptions { get; init; } = default!;

        public SocketSenderPool SenderPool { get; init; } = default!;

        public MemoryPool<byte> MemoryPool { get; init; } = default!;
    }
}
