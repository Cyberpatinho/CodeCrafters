using codecrafters_bittorrent.src.Objects;
using System.Text.Json.Serialization;
using System.Text.Json;
using codecrafters_bittorrent.src.Utils;

namespace codecrafters_bittorrent.src.Bencoding
{
    public class DecodedObjectConverter : JsonConverter<DecodedObject>
    {
        public override DecodedObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DecodedObject value, JsonSerializerOptions options)
        {
            if (value.Value is byte[] byteArr)
            {
                string utf8String = Helper.ToUTF8(byteArr);
                JsonSerializer.Serialize(writer, utf8String, options);
            }
            else
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
