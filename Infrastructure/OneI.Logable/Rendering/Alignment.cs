namespace OneI.Logable.Rendering;

public readonly struct Alignment
{
    public Alignment(Direction direction, int width)
    {
        Direction = direction;
        Width = width;
    }

    public Direction Direction { get; }

    public int Width { get; }
}
