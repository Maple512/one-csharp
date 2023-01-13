namespace OneI.Logable.RollFile;

using OneI.Logable.Fakes;

public class RollFile_Test
{
    [Fact]
    public async void roll_file_write()
    {
        var logger = Fake.CreateLogger("{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}",roll: options =>
        {
            options.Frequency = RollFrequency.Hour;
        });

        logger.Error(" de {SourceContext} bug ");

        await Task.Delay(3 * 1000);

        logger.Error(" de {SourceContext} bug ");
    }
}
