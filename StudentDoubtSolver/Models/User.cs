using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentDoubtSolver.Models
{
    public class User
    {
        [Key] // Tells Entity Framework this is the Primary Key
        public int Id { get; set; }

        // I added UserName so we have a name to show on the Profile page!
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // --- NEW PROFILE DASHBOARD PROPERTIES ---
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public int ScholarPoints { get; set; }

        // Navigation Properties (Optional but helpful for EF Core)
        // This allows a User to have many Questions and many Answers
        public List<Question>? Questions { get; set; }
        public List<Answers>? Answers { get; set; }
    }
}