using codecrafters_bittorrent.src.Objects;
using codecrafters_bittorrent.src.Bencoding;
using System.Text.Json.Serialization;
using System.Text.Json;
using codecrafters_bittorrent.src.Utils;
using codecrafters_bittorrent.Utils;
using codecrafters_bittorrent.src.Attributes;
using System.Reflection;

namespace codecrafters_bittorrent.src.Services
{
    public class TorrentService
    {
        private Torrent Metainfo { get; set; }
        public TorrentService(string filepath)
        {
            if (File.Exists(filepath))
            {
                byte[] bytes = File.ReadAllBytes(filepath);
                var decodedObject = Bencode.Decode(bytes);

                Metainfo = decodedObject.Value is Dictionary<string, DecodedObject> data
                    ? TorrentMapper.Map<Torrent>(data)
                    : throw new InvalidDataException($"Error: No metainfo dictionary wrapper for {Path.GetFileName(filepath)}.");
            }
            else
                throw new FileNotFoundException($"Error: No file found for path: {filepath}.");
        }

        private string? GetInfo<T>(T obj, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            var displayAttribute = propertyInfo?.GetCustomAttribute<DisplayAttribute>();

            var displayName = displayAttribute?.DisplayName;
            var value = propertyInfo?.GetValue(obj);

            if (displayName == null || value == null)
                return null;

            return $"{displayName}: {value}";
        }

        public string GetInfo(string propertyName)
        {
            string? result = GetInfo(Metainfo, propertyName) ?? 
                GetInfo(Metainfo.Info, propertyName);

            if (result == null)
                throw new InvalidOperationException($"Error: Unknown property {propertyName} or value not set.");

            return result;
        }
    }
}
