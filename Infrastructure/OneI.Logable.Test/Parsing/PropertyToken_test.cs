namespace OneI.Logable.Parsing;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PropertyToken_test
{
    [Fact]
    public void int_try_parse_number()
    {
        var result = int.TryParse("6", NumberStyles.None, CultureInfo.InvariantCulture, out var number);
    }
}
