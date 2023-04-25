namespace OneI.Generators;

public static class CorrelationIdGenerator
{
    private static readonly char[] encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
    private static long _lastId = DateTimeOffset.UtcNow.Ticks;

    public static string NextId() => GenerateId(Interlocked.Increment(ref _lastId));

    private static string GenerateId(long id)
    {
        return string.Create(13, id, (buffer, value) =>
        {
            var chars = encode32Chars;

            buffer[12] = chars[value & 31];
            buffer[11] = chars[(value >> 5) & 31];
            buffer[10] = chars[(value >> 10) & 31];
            buffer[9] = chars[(value >> 15) & 31];
            buffer[8] = chars[(value >> 20) & 31];
            buffer[7] = chars[(value >> 25) & 31];
            buffer[6] = chars[(value >> 30) & 31];
            buffer[5] = chars[(value >> 35) & 31];
            buffer[4] = chars[(value >> 40) & 31];
            buffer[3] = chars[(value >> 45) & 31];
            buffer[2] = chars[(value >> 50) & 31];
            buffer[1] = chars[(value >> 55) & 31];
            buffer[0] = chars[(value >> 60) & 31];
        });
    }
}
