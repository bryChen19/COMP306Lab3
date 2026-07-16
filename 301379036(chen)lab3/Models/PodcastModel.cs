using System.ComponentModel.DataAnnotations;

namespace _301379036_chen_lab3.Models
{
    public class PodcastModel
    {
        [Key]
        public int PodcastID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
