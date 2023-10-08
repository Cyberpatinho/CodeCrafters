using codecrafters_bittorrent.src.Enum;
using codecrafters_bittorrent.src.Objects;
using System.Text;
using System.Text.Json;

namespace codecrafters_bittorrent.src.Encoding
{
    public static class Bencode
    {
        #region Constants
        private const char INTEGER_START = 'i';
        private const char INTEGER_END = 'e';
        private const char LIST_START = 'l';
        private const char LIST_END = 'e';
        #endregion

        #region Encoding
        public static string Encode(DecodedObject obj)
        {
            switch (obj.BencodeType)
            {
                case BencodeType.ByteString:
                    return EncodeByteString(obj);
                case BencodeType.Integer:
                    return EncodeInteger(obj);
                default:
                    return EncodeList(obj);
            }
        }

        private static string EncodeByteString(DecodedObject obj)
        {
            var value = ((string)obj.Value);
            var length = (int)value.Length;
            return $"{length}:{value}";
        }

        private static string EncodeInteger(DecodedObject obj)
        {
            return $"{INTEGER_START}{obj.Value}{INTEGER_END}";
        }

        private static string EncodeList(DecodedObject obj)
        {
            var sb = new StringBuilder();
            var list = (List<DecodedObject>)obj.Value;
            foreach (var item in list)
            {
                sb.Append(Encode(item));
            }

            return $"{LIST_START}{sb.ToString()}{LIST_END}";
        }
        #endregion
        
        #region Decoding
        public static DecodedObject Decode(string value)
        {
            var decodingType = GetDecodingType(value);
            switch (decodingType)
            {
                case "String":
                    return new DecodedObject(DecodeString(value), BencodeType.ByteString);
                case "Integer":
                    return new DecodedObject(DecodeInteger(value), BencodeType.Integer);
                case "List":
                    return new DecodedObject(DecodeList(value), BencodeType.List);
                default:
                    throw new NotImplementedException();
            }
        }

        private static string GetDecodingType(string value)
        {
            if (Char.IsDigit(value[0]))
            {
                return "String";
            }
            else if (value[0].Equals(INTEGER_START))
            {
                return "Integer";
            }
            else if (value[0].Equals(LIST_START))
            {
                return "List";
            }
            else
            {
                throw new InvalidOperationException($"Unhandled encoded value: {value}");
            }
        }

        private static string DecodeString(string value)
        {
            var colonIndex = value.IndexOf(':');
            if (colonIndex != -1)
            {
                var strLength = int.Parse(value[..colonIndex]);
                var result = value.Substring(colonIndex + 1, strLength);
                return result;
            }
            else
            {
                throw new InvalidOperationException($"Invalid encoded value for String: {value}");
            }
        }

        private static long DecodeInteger(string value)
        {
            var endIndex = value.IndexOf(INTEGER_END);
            if (endIndex != -1)
            {
                var integerPart = value.Substring(1, endIndex - 1);
                if (long.TryParse(integerPart, out long result))
                {
                    return result;
                }
                else
                {
                    throw new InvalidDataException($"Invalid integer value: {integerPart}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Invalid encoded value for Integer: {value}");
            }
        }

        private static List<DecodedObject> DecodeList(string value)
        {
            var result = new List<DecodedObject>();

            value = value[1..];
            while (value[0] != LIST_END)
            {
                var item = Decode(value);
                result.Add(item);
                
                var offset = Encode(item).Length;

                value = value[offset..];
            }

            return result;
        }

        #endregion
    }
}
