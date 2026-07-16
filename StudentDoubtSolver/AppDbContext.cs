using Microsoft.EntityFrameworkCore;
using StudentDoubtSolver.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }

    // ADD THESE TWO LINES TO REMOVE THE RED ERRORS:
    public DbSet<Answers> Answers { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This line kills the 'UserId1' error forever
        modelBuilder.Entity<Question>()
            .Property(q => q.UserId)
            .HasColumnName("UserId");

        modelBuilder.Entity<Answers>()
            .Property(a => a.UserId)
            .HasColumnName("UserId");
    }
}