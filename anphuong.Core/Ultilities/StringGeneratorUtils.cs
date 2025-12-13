using System.Security.Cryptography;

namespace anphuong.Core.Ultilities
{
    public class StringGeneratorUtils
    {
        public static string GenerateRandomUsername()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var bytes = new byte[6];
            RandomNumberGenerator.Fill(bytes);

            char[] result = new char[6];
            for (int i = 0; i < 6; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return "anphuong_" + new string(result);
        }
    }
}
