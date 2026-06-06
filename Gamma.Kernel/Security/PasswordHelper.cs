using System;
using System.Security.Cryptography;
using System.Text;

namespace Gamma.Kernel.Security;

public static class PasswordHelper
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

    public static byte[] CreateDbPassword(string password, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password + userId),
            salt,
            Iterations,
            _hashAlgorithm,
            HashSize);

        byte[] result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

        return result;
    }

    public static bool Verify(byte[] storedPassword, string password, string userId)
    {
        if (storedPassword == null || storedPassword.Length != SaltSize + HashSize)
            return false;

        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userId))
            return false;

        byte[] salt = new byte[SaltSize];
        Buffer.BlockCopy(storedPassword, 0, salt, 0, SaltSize);

        byte[] expectedHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password + userId),
            salt,
            Iterations,
            _hashAlgorithm,
            HashSize);

        ReadOnlySpan<byte> actualHash = storedPassword.AsSpan(SaltSize, HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
    private static readonly char[] Alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public static string GenerateRandomPassword(int length, int nonAlphaNumCount)
    {
        if (length < 8 || length > 128) throw new ArgumentOutOfRangeException(nameof(length));
        if (nonAlphaNumCount < 0 || nonAlphaNumCount > length) throw new ArgumentOutOfRangeException(nameof(nonAlphaNumCount));

        char[] password = new char[length];

        // 1. پر کردن بخش غیر الفانامریک
        for (int i = 0; i < nonAlphaNumCount; i++)
        {
            password[i] = Punctuations[RandomNumberGenerator.GetInt32(Punctuations.Length)];
        }

        // 2. پر کردن باقی‌مانده با حروف و اعداد
        for (int i = nonAlphaNumCount; i < length; i++)
        {
            password[i] = Alphanumeric[RandomNumberGenerator.GetInt32(Alphanumeric.Length)];
        }

        // 3. درهم‌ریختن (Shuffle) آرایه برای امنیت بیشتر (Fisher-Yates Shuffle)
        RandomNumberGenerator.Shuffle(password.AsSpan());

        return new string(password);
    }
}