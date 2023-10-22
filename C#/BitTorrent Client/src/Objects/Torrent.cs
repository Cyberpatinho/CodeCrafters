using codecrafters_bittorrent.src.Attributes;

namespace codecrafters_bittorrent.src.Objects
{
    public class Torrent
    {
        [Name("announce")]
        [Display("Tracker URL")]
        public string? Announce { get; set; }
        [Name("info")]
        public TorrentInfo Info { get; set; }

        public Torrent()
        {
            Info = new TorrentInfo();
        }

        public class TorrentInfo
        {
            [Name("length")]
            [Display("Length")]
            public long Length { get; set; } = 0;
            [Name("name")]
            public string? Name { get; set; }
            [Name("piece length")]
            public long PieceLength { get; set; } = 0;
            [Name("pieces")]
            public string? Pieces { get; set; }
        }
    }
}
