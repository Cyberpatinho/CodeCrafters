using codecrafters_bittorrent.src.Enum;
using codecrafters_bittorrent.src.Extensions;
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
        public static DecodedObject Decode(byte[] encodedObj)
        {
            BencodeType type = GetDecodingType(encodedObj);
            switch (type)
            {
                case BencodeType.ByteString:
                    return new DecodedObject(
                        DecodeByteString(encodedObj), 
                        BencodeType.ByteString);

                case BencodeType.Integer:
                    return new DecodedObject(
                        DecodeInteger(encodedObj.Skip(1).ToArray()), 
                        BencodeType.Integer);

                case BencodeType.List:
                    return new DecodedObject(
                        DecodeList(encodedObj.Skip(1).ToArray())
                        , BencodeType.List);

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

            string encodedInt = Helper.ToUTF8(encodedObj.Take(endIdx).ToArray());

            if (!long.TryParse(encodedInt, out long result))
                throw new InvalidOperationException($"Error: {encodedInt} is not a valid Integer representation.");

            return result;
        }

        private static List<DecodedObject> DecodeList(byte[] encodedObj)
        {
            var result = new List<DecodedObject>();

            int idx = 0;
            while (encodedObj[idx] != LIST_END)
            {
                var item = Decode(encodedObj);
                result.Add(item);

                int offset = Encode(item).Length;

                encodedObj = encodedObj.Skip(offset).ToArray();
            }

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
            else if (firstByte.Equals(LIST_START))
            {
                return BencodeType.List;
            }
            else
            {
                throw new InvalidOperationException($"Unhandled encoding: {Helper.ToUTF8(encodedObj)}.");
            }
        }

        #endregion

        #region Encoding
        private static byte[] Encode(DecodedObject decodedObj)
        {
            switch (decodedObj.BencodeType)
            {
                case BencodeType.ByteString:
                    return EncodeByteString(decodedObj);
                case BencodeType.Integer:
                    return EncodeInteger(decodedObj);
                case BencodeType.List:
                    return EncodeList(decodedObj);
                default:
                    throw new InvalidOperationException($"Error: Unable to encode {decodedObj.Value} of type {decodedObj.BencodeType}");
            }
        }

        private static byte[] EncodeByteString(DecodedObject decodedObj)
        {
            var buffer = new MemoryStream();

            if (decodedObj.Value is byte[] byteArr)
            {
                string utf8Len = byteArr.Length.ToString();
                buffer.Append(Helper.ToByteArray(utf8Len));
                buffer.Append(LENGTH_SEPARATOR);
                buffer.Append(byteArr);
            }
            else
                throw new InvalidOperationException($"Error: Encoding object was not of type: {typeof(byte[])}");


            return buffer.ToArray();
        }

        private static byte[] EncodeInteger(DecodedObject decodedObj)
        {
            var buffer = new MemoryStream();

            if (decodedObj.Value is long integer)
            {
                string utf8Integer = integer.ToString();

                buffer.Append(INTEGER_START);
                buffer.Append(Helper.ToByteArray(utf8Integer));
                buffer.Append(INTEGER_END);
            }
            else 
                throw new InvalidOperationException($"Error: Encoding object was not of type: {typeof(long)}");

            return buffer.ToArray();
        }

        private static byte[] EncodeList(DecodedObject decodedObj)
        {
            var buffer = new MemoryStream();
            if (decodedObj.Value is List<DecodedObject> list)
            {
                buffer.Append(LIST_START);

                foreach (var obj in list)
                {
                    buffer.Append(Encode(obj));
                }

                buffer.Append(LIST_END);
            }
            else 
                throw new InvalidOperationException($"Error: Encoding object was not of type: {typeof(List<DecodedObject>)}");

            return buffer.ToArray();
        }

        #endregion
    }
}
