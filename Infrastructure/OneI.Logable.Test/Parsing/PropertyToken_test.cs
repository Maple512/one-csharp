namespace OneI.Logable.Parsing;
using System.Globalization;

public class PropertyToken_test
{
    [Fact]
    public void int_try_parse_number()
    {
        var result = int.TryParse("6", NumberStyles.None, CultureInfo.InvariantCulture, out var number);
    }
}
