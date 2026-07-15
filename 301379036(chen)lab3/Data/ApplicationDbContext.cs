using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _301379036_chen_lab3.Models;

namespace _301379036_chen_lab3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<EpisodeModel> Episodes { get; set; }
        public DbSet<PodcastModel> Podcasts { get; set; }
        public DbSet<SubscriptionModel> Subscriptions { get; set; }
    }
}