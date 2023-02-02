namespace OneI.Logable;

public class LogFileOptions
{
    public LogFileOptions(string path)
    {
        var file = new FileInfo(path);

        if(file.Directory is not { Exists: true })
        {
            System.IO.Directory.CreateDirectory(file.DirectoryName!);
        }

        Path = file.FullName;
        Directory = file.DirectoryName!;
        FileName = System.IO.Path.GetFileNameWithoutExtension(file.Name);
        FileExtensions = file.Extension;
    }

    public string Path { get; }
    public string Directory { get; }
    public string FileName { get; }
    public string FileExtensions { get; }

    public IFormatProvider? FormatProvider { get; set; }

    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// 缓冲区长度，默认：32KB
    /// </summary>
    public int BufferSize { get; set; } = 32768;// 32kb

    /// <summary>
    /// 文件预分配长度，单位: 字节
    /// </summary>
    public long PreallocationSize { get; set; }
}

public class LogRollFileOptions : LogFileOptions
{
    public LogRollFileOptions(string path) : base(path)
    {
    }

    /// <summary>
    ///     文件最大长度（单位：MB）
    /// </summary>
    public long SizeLimit { get; set; }

    /// <summary>
    ///     文件滚动周期，默认：<see cref="RollFrequency.Hour" />
    /// </summary>
    public RollFrequency Frequency { get; set; } = RollFrequency.Hour;

    /// <summary>
    ///     数量限制
    /// </summary>
    public int CountLimit { get; set; }

    /// <summary>
    ///     文件过期时间
    /// </summary>
    public TimeSpan ExpiredTime { get; set; }
}
