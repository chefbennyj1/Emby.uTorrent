namespace uTorrent.Utils
{
    public enum MediaType
    {
        MOVIE,
        SERIES,
        UNKNOWN
    }

    public class MediaInfo
    {
        public string SortName { get; set; }
        public int? Season { get; set; }
        public int? Episode { get; set; }
        public MediaType MediaType { get; set; } = MediaType.UNKNOWN;
        public string Resolution { get; set; }
        public int Year { get; set; }

    }

}