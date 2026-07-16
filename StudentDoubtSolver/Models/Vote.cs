using System.ComponentModel.DataAnnotations;
namespace StudentDoubtSolver.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public int UserId { get; set; }
        public int Value { get; set; } // 1 for upvote, -1 for downvote
    }
}