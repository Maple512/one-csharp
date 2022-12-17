namespace OneI.Textable;

using OneI.Textable.Rendering;

public class Rendering_Test
{
    [Fact]
    public void text_align()
    {
        Render(new(12), "1212454").ShouldBe("     1212454");
        Render(new(-12), "123").ShouldBe("123         ");

#pragma warning disable IDE0055
                Render(new(-12), "id", "name").ShouldBe("id          name        ");
             Render(new(-12), "1", "Maple512").ShouldBe("1           Maple512    ");
             Render(new(-12), "2", "Maple514").ShouldBe("2           Maple514    ");
             Render(new(-12), "3", "Maple516").ShouldBe("3           Maple516    ");
        Render(new(-12), "100", "Mapl565e516").ShouldBe("100         Mapl565e516 ");
#pragma warning restore IDE0055
    }

    private static string Render(Alignment? alignment, params string[] text)
    {
        var writer = new StringWriter();

        foreach(var item in text)
        {
            RenderHelper.Padding(writer, item, alignment);
        }

        return writer.ToString();
    }
}
