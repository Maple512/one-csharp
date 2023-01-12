namespace OneI.Horology;
// source from: https://github.com/AndreyAkinshin/perfolizer
public readonly struct UnitPresentation
{
    public static readonly UnitPresentation Default = new(true, 0);
    public static readonly UnitPresentation Invisible = new(false, 0);

    public bool IsVisible { get; }

    public int MinUnitWidth { get; }

    public UnitPresentation(bool isVisible, int minUnitWidth)
    {
        IsVisible = isVisible;
        MinUnitWidth = minUnitWidth;
    }

    public static UnitPresentation FromVisibility(bool isVisible) => new(isVisible, 0);

    public static UnitPresentation FromWidth(int unitWidth) => new(true, unitWidth);
}
