using codecrafters_bittorrent.src.Enum;
using codecrafters_bittorrent.src.Objects;
using System.Text;

namespace codecrafters_bittorrent.src.Encoding
{
    public static class Bencode
    {
        #region Constants
        private const char INTEGER_START = 'i';
        private const char INTEGER_END = 'e';
        private const char LIST_START = 'l';
        private const char LIST_END = 'e';
        private const char DICTIONARY_START = 'd';
        private const char DICTIONARY_END = 'e';
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
                case BencodeType.List:
                    return EncodeList(obj);
                case BencodeType.Dictionary:
                    return EncodeDictionary(obj);
                default:
                    throw new InvalidOperationException("Invalid object for encoding. ");
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

            return $"{LIST_START}{sb}{LIST_END}";
        }

        private static string EncodeDictionary(DecodedObject obj)
        {
            var sb = new StringBuilder();
            var dict = (Dictionary<string, DecodedObject>)obj.Value;
            foreach (var (key, value) in dict)
            {
                sb.Append(Encode(new DecodedObject(key, BencodeType.ByteString)) + Encode(value));
            }

            return $"{DICTIONARY_START}{sb}{DICTIONARY_END}";
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
                case "Dictionary":
                    return new DecodedObject(DecodeDictionary(value), BencodeType.Dictionary);
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
            else if (value[0].Equals(DICTIONARY_START))
            {
                return "Dictionary";
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
                var integerPart = value[1..endIndex];
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

        private static Dictionary<string, DecodedObject> DecodeDictionary(string value)
        {
            var result = new Dictionary<string, DecodedObject>();

            value = value[1..];
            while (value[0] != DICTIONARY_END)
            {
                var dictKey = Decode(value);
                var offset = Encode(dictKey).Length;
                value = value[offset..];

                if (dictKey.BencodeType != BencodeType.ByteString)
                    throw new InvalidDataException($"Dictionary key {dictKey.Value} is not a ByteString.");

                var dictValue = Decode(value);
                offset = Encode(dictValue).Length;
                value = value[offset..];

                result.Add((string)dictKey.Value, dictValue);
            }

            return result;
        }

        #endregion
    }
}
