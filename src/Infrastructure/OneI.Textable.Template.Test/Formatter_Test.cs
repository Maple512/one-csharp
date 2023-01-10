namespace OneI.Textable;

public class Formatter_Test
{
    [Fact]
    public void custom_formatter()
    {
        TextTemplate.Create("{Date:yyyy-MM-dd}")
            .AddProperty("Date", new DateTime(2022, 12, 17), new DateFormatter())
        .ToString().ShouldBe("2022-12-17 00:00");
    }

    private class DateFormatter : IFormatter<DateTime>
    {
        public void Format(DateTime value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
        {
            writer.Write($"{value:yyyy-MM-dd HH:mm}");
        }
    }
}
