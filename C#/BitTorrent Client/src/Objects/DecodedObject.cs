using codecrafters_bittorrent.src.Enum;
using System.Text.Json.Serialization;

namespace codecrafters_bittorrent.src.Objects
{
    public class DecodedObject
    {
        [JsonIgnore]
        public BencodeType BencodeType { get; set; }
        public object Value { get; set; }
        public DecodedObject(object value, BencodeType bencodeType)
        {
            BencodeType = bencodeType;
            Value = value;
        }
    }
}
