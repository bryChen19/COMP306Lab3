using System.ComponentModel.DataAnnotations;

namespace _301379036_chen_lab3.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public enum Role { Podcaster, Listener, Admin }
    }
}
