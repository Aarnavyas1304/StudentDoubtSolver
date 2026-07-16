using System.ComponentModel.DataAnnotations.Schema;

namespace StudentDoubtSolver.Models
{
    public class Answers
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}