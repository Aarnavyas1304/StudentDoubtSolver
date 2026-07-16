using System.Collections.Generic;

namespace StudentDoubtSolver.Models
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        // Stats for our Bubbly UI
        public int ScholarPoints { get; set; }
        public int QuestionsCount { get; set; }
        public int AnswersCount { get; set; }

        // For the Activity Feed Timeline
        public List<Question>? RecentActivity { get; set; }
    }
}