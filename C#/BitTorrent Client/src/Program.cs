using codecrafters_bittorrent.src.Bencode;
using codecrafters_bittorrent.src.Enum;
using codecrafters_bittorrent.src.Objects;
using codecrafters_bittorrent.src.Services;
using System.Text.Json;

// Parse arguments
var (command, param) = args.Length switch
{
    0 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    1 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    _ => (args[0], args[1])
};

if (command == "decode")
{
    var options = new JsonSerializerOptions();
    options.Converters.Add(new DecodedObjectConverter());

    byte[] bytes = Helper.ToByteArray(param);
    var decodedObject = Bencode.Decode(bytes);
    Console.WriteLine(JsonSerializer.Serialize(decodedObject, options));
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}
