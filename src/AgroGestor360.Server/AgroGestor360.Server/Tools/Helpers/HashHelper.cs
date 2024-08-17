using System.Security.Cryptography;
using System.Text;

namespace AgroGestor360.Server.Tools.Helpers;

public class HashHelper
{
    public static string GenerateHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        return sb.ToString();
    }
}
