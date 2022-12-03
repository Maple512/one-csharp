namespace OneI.Logable.Templating.Rendering;

public readonly struct Alignment
{
    public Alignment(int width)
    {
        Direction = width >= 0 ? Direction.Right : Direction.Left;
        Width = width;
    }

    public Direction Direction { get; }

    public int Width { get; }

    public override string ToString() => $"{Direction}, {Width}";
}
