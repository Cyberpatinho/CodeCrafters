using codecrafters_bittorrent.src.Bencoding;
using codecrafters_bittorrent.src.Services;
using codecrafters_bittorrent.src.Utils;
using System.Text.Json;

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
else if (command == "info")
{
    var torrentService = new TorrentService(param);

    Console.WriteLine(torrentService.GetInfo("Announce"));
    Console.WriteLine(torrentService.GetInfo("Length"));
    Console.WriteLine(torrentService.GetInfo("Hash"));
    Console.WriteLine(torrentService.GetInfo("PieceLength"));
    Console.WriteLine(torrentService.GetInfo("Pieces"));
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}
