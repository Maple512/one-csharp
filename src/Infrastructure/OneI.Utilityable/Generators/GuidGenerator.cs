//-----------------------------------------------------------------------
// <copyright file="SequentialGuid.cs" company="Jeremy H. Todd">
//     Copyright © Jeremy H. Todd 2011
// </copyright>
//-----------------------------------------------------------------------

namespace OneI.Generators;

using System.Security.Cryptography;

/// <summary>
/// Guid生成器
/// </summary>
/// <remarks>
/// 代码来源：https://github.com/jhtodd/SequentialGuid/blob/27faa6040f051676c6a025ade0c0e3b66bbe6b0b/SequentialGuid/Classes/SequentialGuid.cs
/// 相关文章：https://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database
/// </remarks>
public static class GuidGenerator
{
    /// <summary>
    /// Describes the type of a sequential GUID value.
    /// </summary>
    public enum SequentialGuidType
    {
        /// <summary>
        /// The GUID should be sequential when formatted using the <see cref="Guid.ToString()"/>
        /// method. Used by MySql and PostgreSql.
        /// </summary>
        SequentialAsString,

        /// <summary>
        /// The GUID should be sequential when formatted using the <see cref="Guid.ToByteArray"/>
        /// method. Used by Oracle.
        /// </summary>
        SequentialAsBinary,

        /// <summary>
        /// The sequential portion of the GUID should be located at the end of the Data4 block. Used
        /// by SqlServer.
        /// </summary>
        SequentialAtEnd,
    }

    private static readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

    /// <summary>
    /// <para>针对数据库创建顺序Guid</para>
    /// <para>如果不是数据库排序需要，则不需要调用这个方法</para>
    /// <para>仅支持： <c>MySql、PostgreSql、Oracle、SqlServer</c></para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Guid Create(SequentialGuidType type = SequentialGuidType.SequentialAtEnd)
    {
        // We start with 16 bytes of cryptographically strong random data.
        var randomBytes = new byte[10];

        _randomNumberGenerator.GetBytes(randomBytes);

        // An alternate method: use a normally-created GUID to get our initial random data: byte[]
        // randomBytes = Guid.NewGuid().ToByteArray(); This is faster than using
        // RNGCryptoServiceProvider, but I don't recommend it because the .NET Framework makes no
        // guarantee of the randomness of GUID data, and future versions (or different
        // implementations like Mono) might use a different method.

        // Now we have the random basis for our GUID. Next, we need to create the six-byte block
        // which will be our timestamp.

        // We start with the number of milliseconds that have elapsed since DateTime.MinValue. This
        // will form the timestamp. There's no use being more specific than milliseconds, since
        // DateTime.Now has limited resolution.

        // Using millisecond resolution for our 48-bit timestamp gives us about 5900 years before
        // the timestamp overflows and cycles. Hopefully this should be sufficient for most
        // purposes. :)
        var timestamp = DateTime.UtcNow.Ticks / 10_000L;

        // Then get the bytes
        var timestampBytes = BitConverter.GetBytes(timestamp);

        // Since we're converting from an Int64, we have to reverse on little-endian systems.
        if(BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        var guidBytes = new byte[16];

        switch(type)
        {
            case SequentialGuidType.SequentialAsString:
            case SequentialGuidType.SequentialAsBinary:

                // For string and byte-array version, we copy the timestamp first, followed by the
                // random data.
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                // If formatting as a string, we have to compensate for the fact that .NET regards
                // the Data1 and Data2 block as an Int32 and an Int16, respectively. That means that
                // it switches the order on little-endian systems. So again, we have to reverse.
                if(type == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                {
                    Array.Reverse(guidBytes, 0, 4);
                    Array.Reverse(guidBytes, 4, 2);
                }

                break;

            case SequentialGuidType.SequentialAtEnd:

                // For sequential-at-the-end versions, we copy the random data first, followed by
                // the timestamp.
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                break;
        }

        return new Guid(guidBytes);
    }
}
