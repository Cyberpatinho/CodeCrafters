using System.Text.Json;

namespace codecrafters_bittorrent.src.Encoding
{
    public static class BEncode
    {
        #region Constants
        private const char INTEGER_START = 'i';
        private const char INTEGER_END = 'e';
        private const char LIST_START = 'l';
        private const char LIST_END = 'e';
        #endregion

        #region Decoding
        public static string Decode(string value)
        {
            var decodingType = GetDecodingType(value);
            switch (decodingType)
            {
                case "String":
                    return DecodeString(value);
                case "Integer":
                    return DecodeInteger(value);
                case "List":
                    return DecodeList(value);
                default:
                    throw new NotImplementedException();
            }
        }

        private static string DecodeList(string value)
        {
            var lastIndex = value.LastIndexOf(LIST_END);
            if (lastIndex != -1)
            {

            }
            else
            {
                throw new InvalidOperationException($"Invalid encoded value for List: {value}");
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
            {;
                var strLength = int.Parse(value[..colonIndex]);
                var strValue = value.Substring(colonIndex + 1, strLength);
                return JsonSerializer.Serialize(strValue);
            }
            else
            {
                throw new InvalidOperationException($"Invalid encoded value for String: {value}");
            }
        }

        private static string DecodeInteger(string value)
        {
            var endIndex = value.IndexOf(INTEGER_END);
            if (endIndex != -1)
            {
                var integerPart = value.Substring(1, endIndex - 1);
                if (long.TryParse(integerPart, out long result))
                {
                    return JsonSerializer.Serialize(result);
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

        #endregion
    }
}
