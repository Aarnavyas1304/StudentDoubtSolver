using StudentDoubtSolver.Models;
using System.Collections.Generic;

namespace StudentDoubtSolver.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public int ScholarPoints { get; set; }
        public int QuestionsCount { get; set; }
        public int AnswersCount { get; set; }
        public List<Question> RecentActivity { get; set; }
    }
}