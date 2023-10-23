using codecrafters_bittorrent.src.Objects;
using codecrafters_bittorrent.src.Bencoding;
using codecrafters_bittorrent.src.Utils;
using codecrafters_bittorrent.Utils;
using codecrafters_bittorrent.src.Attributes;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace codecrafters_bittorrent.src.Services
{
    public class TorrentService
    {
        private DecodedObject _decodedTorrent;
        private Torrent _metainfo { get; set; }

        [Display("Info Hash")]
        public string Hash { get; set; }

        public TorrentService(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || File.Exists(filepath))
            {
                _decodedTorrent = DecodeTorrent(filepath);
            }
            else
                throw new FileNotFoundException($"Error: No file found for path: {filepath}.");

            _metainfo = LoadMetainfo();
            Hash = CalculateHash();
        }

        private DecodedObject DecodeTorrent(string filepath)
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            return Bencode.Decode(bytes);
        }

        private Torrent LoadMetainfo()
        {
            return _decodedTorrent.Value is Dictionary<string, DecodedObject> data
                ? TorrentMapper.Map<Torrent>(data)
                : throw new InvalidDataException($"Error: No dictionary wrapper in metainfo file.");
        }

        private string CalculateHash()
        {
            if (_decodedTorrent.Value is Dictionary<string, DecodedObject> data)
            {
                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] bytes = Bencode.Encode(data["info"]);

                    return Helper.ToHexadecimal(sha1.ComputeHash(bytes));
                }
            }
            else
                throw new InvalidDataException($"Error: No dictionary wrapper in metainfo file.");
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
            string? result = GetInfo(_metainfo, propertyName) ?? 
                GetInfo(_metainfo.Info, propertyName) ??
                GetInfo(this, propertyName);

            if (result == null)
                throw new InvalidOperationException($"Error: Unknown property {propertyName} or value not set.");

            return result;
        }
    }
}
