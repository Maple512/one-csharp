namespace OneI.Textable.Templating;

using System.Globalization;
using OneI.Textable.Rendering;

/// <summary>
/// The property token.
/// </summary>
public class PropertyToken : Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyToken"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="text">The text.</param>
    /// <param name="index">The index.</param>
    /// <param name="position">The position.</param>
    /// <param name="format">The format.</param>
    /// <param name="alignment">The alignment.</param>
    /// <param name="indent">The indent.</param>
    public PropertyToken(
        string name,
        string text,
        int index,
        int position,
        string? format,
        Alignment? alignment = null,
        int? indent = null) : base(position, text)
    {
        Name = name;
        Format = format;
        Index = index;
        Alignment = alignment;
        Indent = indent;

        if(int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out var parameterIndex) &&
           parameterIndex >= 0)
        {
            ParameterIndex = parameterIndex;
        }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the format.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the alignment.
    /// </summary>
    public Alignment? Alignment { get; }

    /// <summary>
    /// Gets the indent.
    /// </summary>
    public int? Indent { get; }

    /// <summary>
    /// Gets the parameter index.
    /// </summary>
    public int? ParameterIndex { get; }

    /// <summary>
    /// Resets the position.
    /// </summary>
    /// <param name="position">The position.</param>
    internal void ResetPosition(int position)
    {
        Position = position;
    }
}
