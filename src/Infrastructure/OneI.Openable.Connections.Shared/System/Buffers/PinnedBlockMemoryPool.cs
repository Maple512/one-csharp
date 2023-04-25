namespace System.Buffers;

internal sealed class PinnedBlockMemoryPool : MemoryPool<byte>
{
    /// <summary>
    /// <see langword="4096"/>是因为大多数操作系统使用4k页面。
    /// </summary>
    private const int _blockSize = 4096;

    /// <summary>
    /// 池块的最大分配块大小，可以租用较大的值，但在使用后会将其丢弃，而不是返回到池中。
    /// </summary>
    public override int MaxBufferSize { get; } = _blockSize;

    public static int BlockSize => _blockSize;

    /// <summary>
    /// 当前池中的块的线程安全集合。
    /// 将预先分配所有块跟踪对象，并将其添加到此集合中。
    /// 当内存被请求时，它首先从这里取出，当它被返回时，它被重新添加。
    /// </summary>
    private readonly ConcurrentQueue<MemoryPoolBlock> _blocks = new ConcurrentQueue<MemoryPoolBlock>();

    private bool _isDisposed;

    private readonly object _disposeSync = new object();

    private const int DefaultSize = -1;

    public override IMemoryOwner<byte> Rent(int size = DefaultSize)
    {
        if(size > _blockSize)
        {
            throw new ArgumentOutOfRangeException($"Cannot allocate more than {_blockSize} bytes in a single buffer");
        }

        if(_isDisposed)
        {
            throw new ObjectDisposedException(nameof(MemoryPool<byte>));
        }

        if(_blocks.TryDequeue(out var block))
        {
            return block;
        }

        return new MemoryPoolBlock(this, BlockSize);
    }

    /// <summary>
    /// Called to return a block to the pool. Once Return has been called the memory no longer belongs to the caller, and
    /// Very Bad Things will happen if the memory is read of modified subsequently. If a caller fails to call Return and the
    /// block tracking object is garbage collected, the block tracking object's finalizer will automatically re-create and return
    /// a new tracking object into the pool. This will only happen if there is a bug in the server, however it is necessary to avoid
    /// leaving "dead zones" in the slab due to lost block tracking objects.
    /// </summary>
    /// <param name="block">The block to return. It must have been acquired by calling Lease on the same memory pool instance.</param>
    internal void Return(MemoryPoolBlock block)
    {
        if(!_isDisposed)
        {
            _blocks.Enqueue(block);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if(_isDisposed)
        {
            return;
        }

        lock(_disposeSync)
        {
            _isDisposed = true;

            if(disposing)
            {
                // Discard blocks in pool
                while(_blocks.TryDequeue(out _))
                {

                }
            }
        }
    }
}
