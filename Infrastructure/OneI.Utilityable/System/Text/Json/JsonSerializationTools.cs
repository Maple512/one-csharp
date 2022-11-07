namespace System.Text.Json;

using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Unicode;

[StackTraceHidden]
[DebuggerStepThrough]
public static class JsonSerializationTools
{
    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的标准实例
    /// </summary>
    public static JsonSerializerOptions StandardInstance { get; } = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),// 解决中文乱码
        AllowTrailingCommas = true,
        WriteIndented = true,
    };

    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的实例， <see
    /// cref="JsonSerializerOptions.PropertyNamingPolicy"/> 是 <see cref="SnakeCaseNamingPolly.SnakeCase"/>
    /// </summary>
    public static JsonSerializerOptions SnakeCaseInstance { get; } = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        AllowTrailingCommas = true,
        WriteIndented = true,
        PropertyNamingPolicy = SnakeCaseNamingPolly.SnakeCase,
    };
}

internal class SnakeCaseNamingPolly : JsonNamingPolicy
{
    public static JsonNamingPolicy SnakeCase { get; } = new SnakeCaseNamingPolly();

    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}
