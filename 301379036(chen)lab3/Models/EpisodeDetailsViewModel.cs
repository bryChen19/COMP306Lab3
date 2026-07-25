namespace _301379036_chen_lab3.Models
{
    public class EpisodeDetailsViewModel
    {
        public EpisodeModel Episode { get; set; } = new EpisodeModel();
        public List<CommentsModel> Comments { get; set; } = new List<CommentsModel>();
        public string? CurrentUserId { get; set; }
        public string? NewComment { get; set; }
    }
}
