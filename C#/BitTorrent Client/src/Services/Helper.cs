using System.Text;

namespace codecrafters_bittorrent.src.Services
{
    public class Helper
    {
        public static string ToUTF8(byte[] arr)
        {
            return Encoding.UTF8.GetString(arr);
        }

        public static byte[] ToByteArray(string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }

        public static bool IsDigit (byte b)
        {
            return b >= (byte)'0' && b <= (byte)'9';
        }
    }
}
