namespace _301379036_chen_lab3.Models
{
    public class CommentsModel
    {
        public int CommentId { get; set; }
        public int EpisodeId { get; set; }
        public int PodcastId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
