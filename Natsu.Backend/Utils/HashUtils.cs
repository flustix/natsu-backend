using System.Security.Cryptography;

namespace Natsu.Backend.Utils;

public static class HashUtils
{
    public static string GetHash(Stream input) => BitConverter.ToString(SHA256.Create().ComputeHash(input)).Replace("-", "").ToLower();
    public static string GetHash(byte[] input) => BitConverter.ToString(SHA256.Create().ComputeHash(input)).Replace("-", "").ToLower();
}
