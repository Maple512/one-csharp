namespace OneI.Logable;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneT.CodeGenerator;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        // The source code to test
        var source = """
            #nullable enable
            namespace Test;

            using System;
            using System.Collections;
            using System.Threading.Tasks;
            using System.Collections.Generic;
            using OneI.Logable;

            public class UserService
            {
                public async Task Register()
                {
                    var user = new User(){Id =1};

                        var a = static () => 3;

                        var a1 = async () => await Task.CompletedTask;
                            var a2 = new []{1,2,3};

                                var a3 = new SturctModel1{ Id = 1 };

            var a4 = await Task.FromResult(1);

                    Log.Debug("",
            await Task.FromCanceled<int>(default),
            await Task.FromResult(1),
                    a,
                    a(),
                    a4,(dynamic)1,
                    a3 with { Id = 2 },
                    a2[1],
                    //a2[..1],
                    nameof(a2),
                    user.Id.ToString(),
                    typeof(UserService),
                    a1,
                    new { Id = 1, FirstName = "James", LastName = "Bond" },
                    a.Invoke(),
                    user.Id,
                    1,
                    new object(),
                    (object)1,
                    default(int),
                    new []{1,2,3});
                }

            [Serializable]
            public class User
            {
                public int Id { get; set; }
            }
            }

            public struct SturctModel1
            {
                public int Id { get; set; }
            }
            #nullable restore
            """;
        return Verify(source, new LoggerCodeGenerator());
    }

    [Fact]
    public async void object_parameter()
    {
        var a1 = await Task.FromResult(1);

        var bb = (dynamic)1;
        var a = new SturctModel1 { Id = 1 };

        var b = a with { Id = 2, };
    }

    public struct SturctModel1
    {
        public int Id { get; set; }
    }
}
