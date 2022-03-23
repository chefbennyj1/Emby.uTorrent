using System;
using System.Text.RegularExpressions;

namespace uTorrent.Utils
{
    public class MediaInfoParser
    {
        public MediaInfo GetMediaInfo(string fileName)
        {
            var regexDate = new Regex(@"\b(19|20|21)\d{2}\b");
            var regexTvShow = new Regex(@"(.*?)\.[Ss](\d{1,2})[Ee](\d{2})\.(.*)|(.*?)\.[Ss](\d{1,2})", RegexOptions.IgnoreCase);
            
            var dateMatch = regexDate.Match(fileName);
            if (dateMatch.Success)
            {
                var testTvShow = new Regex(@"(?:([Ss](\d{1,2})[Ee](\d{1,2})))|(?:([Ss](\d{1,2})))", RegexOptions.IgnoreCase);
                if (testTvShow.Matches(fileName).Count < 1) //It's a movie.
                {
                    return new MediaInfo()
                    {
                        SortName = fileName.Split(new[] { dateMatch.Value }, StringSplitOptions.None)[0].Replace('.', ' ').TrimEnd(),
                        Resolution = GetStreamResolutionFromFileName(fileName),
                        MediaType = MediaType.MOVIE,
                        Year = Convert.ToInt32(dateMatch.Value),
                    };
                }
                
            }

            var tvShowMatchCollection = regexTvShow.Matches(fileName);
           
            foreach (Match match in tvShowMatchCollection)
            {
                int.TryParse(match.Groups[2].Value, out var season);
                int.TryParse(match.Groups[3].Value, out var episode); 
                return new MediaInfo()
                {
                    SortName = string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[5].Value.Replace(".", " ") : match.Groups[1].Value.Replace(".", " "),
                    Resolution = GetStreamResolutionFromFileName(fileName),
                    Season = season,
                    Episode = episode,
                    MediaType = MediaType.SERIES 
                };
            }
            return new MediaInfo(); //Empty
        }
        private static string GetStreamResolutionFromFileName(string sourceFileName)
        {
            var videoResolutionFlags = new[]
            {
                "480p",
                "720p",
                "1080p",
                "2160p", 
                "4K"
            };
            
            foreach(var resolution in videoResolutionFlags)
            {
                if(sourceFileName.Contains(resolution))
                {
                    return resolution;
                }
            }
            return string.Empty;
        }
    }
}
