using Microsoft.EntityFrameworkCore;
using StudentDoubtSolver.Models;

namespace StudentDoubtSolver.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; } // Plural to match your model
        public DbSet<Vote> Votes { get; set; }
    }
}