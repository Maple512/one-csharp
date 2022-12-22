namespace System.Text.Json;

using System.Text.Encodings.Web;
using System.Text.Unicode;
/// <summary>
/// The json serialization tools.
/// </summary>

[StackTraceHidden]
[DebuggerStepThrough]
public static class JsonSerializationTools
{
    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的标准实例
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonSerializerOptions StandardInstance()
    {
        return new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),// 解决中文乱码
            AllowTrailingCommas = true,
            WriteIndented = true,
        };
    }

    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的实例， <see
    /// cref="JsonSerializerOptions.PropertyNamingPolicy"/> 是 <see cref="SnakeCaseNamingPolly.SnakeCase"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonSerializerOptions SnakeCaseInstance()
    {
        return new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            AllowTrailingCommas = true,
            WriteIndented = true,
            PropertyNamingPolicy = SnakeCaseNamingPolly.SnakeCase,
        };
    }
}
/// <summary>
/// The snake case naming polly.
/// </summary>

internal class SnakeCaseNamingPolly : JsonNamingPolicy
{
    /// <summary>
    /// Gets the snake case.
    /// </summary>
    public static JsonNamingPolicy SnakeCase { get; } = new SnakeCaseNamingPolly();

    /// <summary>
    /// Converts the name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A string.</returns>
    public override string ConvertName(string name)
    {
        return name?.ToSnakeCase()!;
    }
}
