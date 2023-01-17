namespace OneI.Logable.Internal;

using Microsoft.Win32.SafeHandles;

/// <summary>
/// 文件队列
/// </summary>
internal class FileQueue : IDisposable
{
    private readonly Queue<FileItem>? _files;
    private readonly int? _countLimit;
    private bool _hasLimit;

    /// <summary>
    /// 构建一个文件队列
    /// </summary>
    /// <param name="countLimit">文件数量上限</param>
    public FileQueue(int? countLimit)
    {
        _hasLimit = countLimit.HasValue && countLimit.Value > 1;
        if(_hasLimit)
        {
            _files = new(countLimit!.Value);
        }

        _countLimit = countLimit;
    }

    /// <summary>
    /// 获取一个新文件，将新文件添加到文件队列
    /// </summary>
    /// <param name="fullPath">文件全路径</param>
    /// <param name="preAllocationSize">初始分配大小（字节）</param>
    /// <returns></returns>
    public SafeFileHandle GetNewFile(
        string fullPath,
        long? preAllocationSize)
    {
        IOTools.EnsureExistedDirectory(fullPath);

        if(_hasLimit && _files!.Count >= _countLimit!.Value)
        {
            DeleteredundantFiles();
        }

        var file = CreateNewFile(fullPath, preAllocationSize ?? 0);

        if(_hasLimit)
        {
            _files!.Enqueue(new FileItem(file, fullPath));
        }

        return file;
    }

    public static SafeFileHandle CreateNewFile(string fullPath, long preAllocationSize = 0)
    {
        return File.OpenHandle(
           fullPath,
           FileMode.Create, // 创建新文件，已存在则覆盖
           FileAccess.Write,
           FileShare.Read,// 仅共享读权限
           System.IO.FileOptions.Asynchronous | System.IO.FileOptions.RandomAccess,
           preAllocationSize);
    }

    /// <summary>
    /// 移除多余文件（设置了数量上限）
    /// </summary>
    private void DeleteredundantFiles()
    {
        if(_hasLimit)
        {
            var count = _files!.Count - _countLimit;
            for(var i = 0; i < count; i++)
            {
                var file = _files.Dequeue();

                try
                {
                    File.Delete(file.FullPath);
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// 移除过期文件
    /// </summary>
    public void DeleteExpiredFiles(DateTime expired)
    {
        if(_hasLimit)
        {
            for(var i = 0; i < _files!.Count; i++)
            {
                var file = _files.Dequeue();

                if(file.CreatedAt > expired)
                {
                    break;
                }

                try
                {
                    File.Delete(file.FullPath);
                }
                catch { }
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if(_hasLimit)
        {
            for(var i = 0; i < _files!.Count; i++)
            {
                var item = _files.Dequeue();
                if(item.File.IsClosed == false)
                {
                    item.File.Close();
                }
            }
        }
    }
}
