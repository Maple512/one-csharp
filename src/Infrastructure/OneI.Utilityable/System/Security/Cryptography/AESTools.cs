namespace System.Security.Cryptography;

using OneI;

/// <summary>
/// The a e s tools.
/// </summary>

[Obsolete]
public static class AESTools
{
    // source: https://github.com/dotnet/runtime/blob/899bf9704693661d6fe53fdb7f737f76447efa14/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/Aes.cs#L36
    private static readonly int[] _keySizes = { 16, 24, 32 };

    /// <summary>
    /// Encrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key.</param>
    /// <param name="iv">The iv.</param>
    /// <param name="keySize">The key size.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>A string.</returns>
    public static string Encrypt(
        string data,
        string key,
        string iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        Check.NotNullOrWhiteSpace(data);

        var result = Encrypt(
            Encoding.UTF8.GetBytes(data),
            Encoding.UTF8.GetBytes(key),
            Encoding.UTF8.GetBytes(iv),
            keySize,
            mode,
            padding);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Encrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key.</param>
    /// <param name="iv">The iv.</param>
    /// <param name="keySize">The key size.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>An array of byte.</returns>
    public static byte[] Encrypt(
        byte[] data,
        byte[] key,
        byte[] iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        Check.NotNullOrEmpty(data);

        using var aes = Aes.Create();

        aes.Mode = mode;
        aes.KeySize = keySize ?? KeySizeRollback(key);
        aes.Padding = padding;

        var encryptor = aes.CreateEncryptor(key, iv);

        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// Decrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key.</param>
    /// <param name="iv">The iv.</param>
    /// <param name="keySize">The key size.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>A string.</returns>
    public static string Decrypt(
        string data,
        string key,
        string iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        Check.NotNullOrWhiteSpace(data);

        var result = Decrypt(
            Convert.FromBase64String(data),
            Encoding.UTF8.GetBytes(key),
            Encoding.UTF8.GetBytes(iv),
            keySize,
            mode,
            padding);

        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// Decrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key.</param>
    /// <param name="iv">The iv.</param>
    /// <param name="keySize">The key size.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>An array of byte.</returns>
    public static byte[] Decrypt(
        byte[] data,
        byte[] key,
        byte[] iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        Check.NotNullOrEmpty(data);

        using var aes = Aes.Create();

        aes.Mode = mode;
        aes.KeySize = keySize ?? KeySizeRollback(key);
        aes.Padding = padding;

        var transform = aes.CreateDecryptor(key, iv);

        return transform.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// 通过指定的key计算KeySize
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static int KeySizeRollback(byte[] key)
    {
        if(_keySizes.Contains(key.Length))
        {
            return key.Length * 8;
        }

        throw new ArgumentOutOfRangeException(nameof(key), "Specified key is not a valid size for this algorithm, the size must be between 16, 24, 32 bytes.");
    }
}
