using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentDoubtSolver.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Votes { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Ensuring this matches the 'Answers' class name
        public virtual ICollection<Answers> Answers { get; set; }
    }
}