namespace OneI.Logable;

using System.IO;
using System.Threading.Tasks;
using OneT.CodeGenerator;
using VerifyXunit;
using Xunit;

/// <summary>
/// The log generator_ test.
/// </summary>
[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    /// <summary>
    /// generator_simples the.
    /// </summary>
    /// <returns>A Task.</returns>
    [Fact]
    public Task generator_simple()
    {
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
                    Log.Error("", "", new object(), new Dictionary<int, int>(), new List<int>());
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

    /// <summary>
    /// object_parameters the.
    /// </summary>
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
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }
    }
}
