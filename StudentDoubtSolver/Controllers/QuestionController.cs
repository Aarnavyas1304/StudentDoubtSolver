using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDoubtSolver.Models;
using System.Linq;

namespace StudentDoubtSolver.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DASHBOARD: Shows the bubbly cards + Search + Answers
        public IActionResult Index(string searchString)
        {
            // Sync Session Name to ViewData so the Header Profile Icon works
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewData["CurrentFilter"] = searchString;

            var query = _context.Questions.Include(q => q.Answers).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
            }

            return View(query.OrderByDescending(q => q.Id).ToList());
        }

        // 2. CREATE (GET): Opens the "Post a Doubt" Page
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // 3. CREATE (POST): Saves the Doubt to SQL
        [HttpPost]
        public IActionResult Create(Question model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            model.UserId = userId.Value;
            model.Votes = 0;

            _context.Questions.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 4. DELETE QUESTION: Removes the entire card
        [HttpPost]
        public IActionResult DeleteQuestion(int id)
        {
            var question = _context.Questions.Include(q => q.Answers).FirstOrDefault(q => q.Id == id);
            if (question != null)
            {
                // Remove associated answers first
                if (question.Answers != null)
                {
                    _context.Answers.RemoveRange(question.Answers);
                }
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 5. DELETE ANSWER: Removes a single solution pill
        [HttpPost]
        public IActionResult DeleteAnswer(int id)
        {
            var answer = _context.Answers.Find(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 6. PROVIDE SOLUTION (GET)
        [HttpGet]
        public IActionResult ProvideSolution(int id)
        {
            var question = _context.Questions.Include(q => q.Answers).FirstOrDefault(q => q.Id == id);
            if (question == null) return NotFound();
            return View(question);
        }

        // 7. POST ANSWER
        [HttpPost]
        public IActionResult PostAnswer(int QuestionId, string Content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var newAns = new Answers { Content = Content, QuestionId = QuestionId, UserId = userId.Value };
            _context.Answers.Add(newAns);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 8. VOTE
        [HttpPost]
        public IActionResult Vote(int id, string type)
        {
            var q = _context.Questions.Find(id);
            if (q != null)
            {
                if (type == "up") q.Votes++; else q.Votes--;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 9. HELPER: Direct Redirect to Profile to ensure clean routing
        public IActionResult MyProfile()
        {
            return RedirectToAction("Index", "Profile");
        }
    }
}