using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDoubtSolver.Models;
using StudentDoubtSolver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.Json;

namespace StudentDoubtSolver.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId);
            if (user == null) return RedirectToAction("Login", "Account");

            // 1. Fetch Basic Stats
            var userQuestions = _context.Questions.Where(q => q.UserId == userId).ToList();
            var userAnswers = _context.Answers.Where(a => a.UserId == userId).ToList();

            int currentStreak = 0;
            string heatmapJson = "{}";

            // 2. SCHOLAR STREAK LOGIC
            try
            {
                // We cast to List to ensure the query executes and we can debug counts
                var logs = _context.ActivityLogs
                    .Where(a => a.UserId == userId)
                    .ToList();

                var activityDates = logs
                    .Select(a => a.ActivityDate.Date)
                    .Distinct()
                    .OrderByDescending(d => d)
                    .ToList();

                DateTime today = DateTime.Today;

                if (activityDates.Any())
                {
                    if (activityDates[0] == today || activityDates[0] == today.AddDays(-1))
                    {
                        currentStreak = 1;
                        for (int i = 0; i < activityDates.Count - 1; i++)
                        {
                            if ((activityDates[i] - activityDates[i + 1]).Days == 1)
                                currentStreak++;
                            else
                                break;
                        }
                    }
                }

                // 3. HEATMAP DATA (Last 90 days for better visual)
                var ninetyDaysAgo = today.AddDays(-90);
                var heatmapData = logs
                    .Where(a => a.ActivityDate >= ninetyDaysAgo)
                    .GroupBy(a => a.ActivityDate.Date)
                    .ToDictionary(
                        g => g.Key.ToString("yyyy-MM-dd"),
                        g => g.Count()
                    );

                heatmapJson = JsonSerializer.Serialize(heatmapData);

                // DEBUG: This helps us see if any data was actually found
                ViewBag.DebugInfo = $"Logged in as ID: {userId}. Found {logs.Count} activity records.";

                // Prepare demo/chart data for the profile graphs
                // Bar (Asked vs Solved)
                var questionsCount = _context.Questions.Count(q => q.UserId == userId);
                var answersCount = _context.Answers.Count(a => a.UserId == userId);
                int[] barData;
                if (questionsCount == 0 && answersCount == 0)
                {
                    // demo fallback so chart always shows something meaningful
                    barData = new[] { 3, 1 };
                }
                else
                {
                    barData = new[] { questionsCount, answersCount };
                }
                ViewBag.BarSolvedJson = JsonSerializer.Serialize(barData);

                // Line (Activity Trend) - last 5 days
                var activityLabels = new List<string>();
                var activityPoints = new List<int>();
                for (int i = 4; i >= 0; i--)
                {
                    var d = today.AddDays(-i);
                    activityLabels.Add(d.ToString("ddd"));
                    var key = d.ToString("yyyy-MM-dd");
                    activityPoints.Add(heatmapData.ContainsKey(key) ? heatmapData[key] : 0);
                }
                // If all activity points are zero, provide demo trend
                if (activityPoints.All(p => p == 0))
                {
                    activityLabels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri" };
                    activityPoints = new List<int> { 8, 12, 6, 10, 9 };
                }
                ViewBag.ActivityLabelsJson = JsonSerializer.Serialize(activityLabels);
                ViewBag.ActivityDataJson = JsonSerializer.Serialize(activityPoints);

                // Performance bar
                var perfVal1 = (user.ScholarPoints + (answersCount * 10)) / 10;
                var perfVal2 = answersCount;
                int[] perf;
                if (perfVal1 == 0 && perfVal2 == 0)
                {
                    perf = new[] { 2, 1 };
                }
                else
                {
                    perf = new[] { perfVal1, perfVal2 };
                }
                ViewBag.BarPerformanceJson = JsonSerializer.Serialize(perf);
            }
            catch (Exception ex)
            {
                ViewBag.DebugInfo = "Database Error: " + ex.Message;
                currentStreak = 0;
                heatmapJson = "{}";
            }

            // 4. Prepare View Model
            string avatarSeed = !string.IsNullOrEmpty(user.AvatarUrl) ? user.AvatarUrl : user.UserName;

            var model = new StudentDoubtSolver.ViewModels.ProfileViewModel
            {
                UserName = user.UserName,
                ScholarPoints = user.ScholarPoints + (userAnswers.Count * 10),
                QuestionsCount = userQuestions.Count,
                AnswersCount = userAnswers.Count,
                RecentActivity = userQuestions.OrderByDescending(q => q.Id).Take(5).ToList()
            };

            ViewBag.Streak = currentStreak;
            ViewBag.HeatmapJson = heatmapJson;
            ViewData["CurrentAvatarSeed"] = avatarSeed;

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAvatar([FromBody] AvatarUpdateRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return BadRequest();

            var user = _context.Users.Find(userId);
            if (user != null && request != null)
            {
                user.AvatarUrl = request.AvatarSeed;
                _context.SaveChanges();
                HttpContext.Session.SetString("UserAvatar", request.AvatarSeed);
                return Ok();
            }
            return BadRequest();
        }
    }

    public class AvatarUpdateRequest
    {
        public string AvatarSeed { get; set; }
    }
}