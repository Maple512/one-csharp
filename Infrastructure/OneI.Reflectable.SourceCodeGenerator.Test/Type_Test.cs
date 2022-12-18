namespace OneI.Reflectable;

public class Type_Test
{
    [Fact]
    public void print_type_information()
    {
        var t = typeof(User);

        var tm = t.Module;
    }

    class User
    {
        public string Name { get; set; }
    }
}
