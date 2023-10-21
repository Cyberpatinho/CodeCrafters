using codecrafters_bittorrent.src.Enum;
using codecrafters_bittorrent.src.Objects;
using codecrafters_bittorrent.src.Services;
using System.Text;

namespace codecrafters_bittorrent.src.Bencode
{
    public static class Bencode
    {
        #region Constants
        private const byte LENGTH_SEPARATOR = (byte)':';
        private const byte INTEGER_START = (byte)'i';
        private const byte INTEGER_END = (byte)'e';
        private const byte LIST_START = (byte)'l';
        private const byte LIST_END = (byte)'e';
        private const byte DICTIONARY_START = (byte)'d';
        private const byte DICTIONARY_END = (byte)'e';
        #endregion

        #region Decoding

        public static DecodedObject Decode(string param)
        {
            // Probably will need to read from file later, since torrent string accepts non-Unicode characters
            byte[] encodedObj = Helper.ToByteArray(param);

            BencodeType type = GetDecodingType(encodedObj);
            switch (type)
            {
                case BencodeType.ByteString:
                    return new DecodedObject(DecodeByteString(encodedObj), BencodeType.ByteString);
                case BencodeType.Integer:
                    return new DecodedObject(DecodeInteger(encodedObj), BencodeType.Integer);
                default:
                    throw new NotImplementedException();
            }
        }

        private static byte[] DecodeByteString(byte[] encodedObj)
        {
            int separatorIdx = Array.IndexOf(encodedObj, LENGTH_SEPARATOR);

            string encodedLen = Helper.ToUTF8(encodedObj.Take(separatorIdx).ToArray());

            if (!int.TryParse(encodedLen, out int len))
                throw new InvalidOperationException($"Error: {encodedLen} is not a valid ByteString length.");

            byte[] result = encodedObj.Skip(separatorIdx + 1).Take(len).ToArray();

            return result;
        }

        private static long DecodeInteger(byte[] encodedObj)
        {
            int endIdx = Array.IndexOf(encodedObj, INTEGER_END);

            string encodedInt = Helper.ToUTF8(encodedObj.Skip(1).Take(endIdx - 1).ToArray());

            if (!long.TryParse(encodedInt, out long result))
                throw new InvalidOperationException($"Error: {encodedInt} is not a valid Integer representation.");

            return result;
        }

        private static BencodeType GetDecodingType(byte[] encodedObj)
        {
            byte firstByte = encodedObj[0];
            if (Helper.IsDigit(firstByte))
            {
                return BencodeType.ByteString;
            }
            else if (firstByte.Equals(INTEGER_START))
            {
                return BencodeType.Integer;
            }
            else
            {
                throw new InvalidOperationException($"Unhandled encoding: {Helper.ToUTF8(encodedObj)}.");
            }
        }

        #endregion

    }
}
