namespace OneI.Logable.RollFile;

using Fakes;

public class RollFile_Test
{
    [Fact]
    
    public void roll_file_write()
    {
        var logger = Fake.CreateLogger("{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}",
            file: options =>
        {
            options.Frequency = RollFrequency.Hour;
        });

        logger.ForContext<RollFile_Test>().Error(" de {SourceContext} bug ");
    }
}
