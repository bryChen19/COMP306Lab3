using System.ComponentModel.DataAnnotations;

namespace _301379036_chen_lab3.Models
{
    public class EpisodeModel
    {
        [Key]
        public int EpisodeId { get; set; }
        public int PodcastId { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public int PlayCount { get; set; }
        public string AudioFileURL { get; set; }
        public int NumberOfViews { get; set; }
        public string Topic { get; set; }
        public string Host { get; set; }
    }
}
