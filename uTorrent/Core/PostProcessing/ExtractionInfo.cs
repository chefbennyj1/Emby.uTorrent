using System;

namespace uTorrent.Core.PostProcessing
{
    public class ExtractionInfo
    {
        public string Name        { get; set; }
        public DateTime completed { get; set; }
        public string size        { get; set; } = string.Empty;
        public string extension   { get; set; } = ".rar";
        public string CopyType    { get; set; } = "Unpacked";
        public double Progress    { get; set; } = 100.0;
    }
}