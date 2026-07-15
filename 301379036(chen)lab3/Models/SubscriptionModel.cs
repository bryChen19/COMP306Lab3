using System.ComponentModel.DataAnnotations;

namespace _301379036_chen_lab3.Models
{
    public class SubscriptionModel
    {
        [Key]
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public int PodcastId { get; set; }
        public DateTime SubscribedDate { get; set; }
    }
}
