using codecrafters_bittorrent.src.Encoding;
using System.Text.Json;

// Parse arguments
var (command, param) = args.Length switch
{
    0 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    1 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    _ => (args[0], args[1])
};

// Parse command and act accordingly
if (command == "decode")
{
    string decodedValue = BEncode.Decode(param);
    Console.WriteLine(decodedValue);
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}
