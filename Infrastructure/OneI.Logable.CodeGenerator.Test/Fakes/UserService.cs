#nullable enable
namespace Tests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using OneI.Logable;

public class UserService
{
    public delegate string Handlers();

    public event Handlers HandlerEvent;

    public enum UserType { a, b, c, d }

    private void Index<T0, T1>(T0 t0, T1 t1)
        where T0 : new()
    {
        var p1 = (object)1;
        var p2 = UserType.a;

        HandlerEvent += () => string.Empty;
        HandlerEvent += () => string.Empty;
        var p3 = HandlerEvent;

        var p4 = 1m;
        var p5 = true;
        var p6 = 'c';
        sbyte p7 = 1;
        var p8 = new object[] { 1, 2, 3, 4, 5 };

        var p9 = new BitArray(new[] { true, false });

        var p10 = new List<int> { 1, 2, 3, 4 };

        var p11 = (1, 2, 3);

        Log.Debug("message text", p1, new object(), p2, UserType.b, p3, p4, 3, p5, false, p6, '0', p7, (sbyte)10, p8, new[] { "123" }, p9, new BitArray(new[] { true, false, }), p10, new List<byte> { 1, 2, 3 }, p11);
    }
}

public static partial class Log
{
    public static void Debug(string message, params object[] args)
    {

    }
}

#nullable restore
