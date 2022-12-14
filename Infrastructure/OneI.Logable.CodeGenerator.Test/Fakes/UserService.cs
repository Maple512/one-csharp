namespace OneI.Logable.Fakes;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The user service.
/// </summary>
public class UserService
{
    public delegate string Handlers();

    public event Handlers HandlerEvent;

    private void Index<T0, T1>(T0 t0, T1 t1)
       where T0 : new()
    {
        var p1 = (object)1;

        HandlerEvent += () => string.Empty;
        HandlerEvent += () => string.Empty;
        var p3 = HandlerEvent;
        var p8 = new object[] { 1, 2, 3, 4, 5 };

        var p9 = new BitArray(new[] { true, false });

        var p10 = new List<int> { 1, 2, 3, 4 };

        var (A, B, C) = (A: 1, B: "", C: 1.3m);

        var p12 = (dynamic)12;

        var p13 = new UserInfo() { Id = 1 };

        var p15 = new Dictionary<int, int> { { 1, 2 }, { 2, 3 }, { 3, 4 } };

        // Log.Debug("message text", p15, p14, p1, new Model1(), p3, "", p13, p8, p12, p9, p2, p10, new object(), UserType.b, p4, 3, p5, false, p6, '0', p7, (sbyte)10, new object[] { "1", 2, 'c' }, new BitArray(new[] { true, false, }), new List<byte> { 1, 2, 3 }, p11, t0, t1);
    }
}
