using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace codecrafters_bittorrent.src.Utils
{
    public class Helper
    {
        public static string ToUTF8(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        
        public static string ToHexadecimal(byte[] bytes) 
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            
            return sb.ToString();
        }

        public static byte[] ToByteArray(string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }

        public static bool IsDigit(byte b)
        {
            return b >= (byte)'0' && b <= (byte)'9';
        }
    }
}
