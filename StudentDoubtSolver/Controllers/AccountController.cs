using Microsoft.AspNetCore.Mvc;
using StudentDoubtSolver.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // --- LOGIN SECTION ---
    [HttpGet]
    public IActionResult Login()
    {
        // If already logged in, send them directly to the discussion feed
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Question");
        }
        return View();
    }

    [HttpPost]
    public IActionResult Login(User model)
    {
        // We check the database using 'Email' and 'Password' to match your Model
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

        if (user != null)
        {
            // SUCCESS: Store the ID in Session so we know who is logged in
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Question");
        }

        // If login fails, send a message back to your "Student Brain Trust" UI
        ViewBag.Message = "Invalid email or password.";
        return View(model);
    }

    // --- REGISTER SECTION ---
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        // 1. Validate incoming model
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 2. Check if the email is already taken
        if (_context.Users.Any(u => u.Email == model.Email))
        {
            ViewBag.Message = "This email is already in use.";
            return View(model);
        }

        // 3. Create the new User object and persist
        var newUser = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            Password = model.Password,
            ScholarPoints = 0
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        TempData["Success"] = "Registration successful! Please login.";
        return RedirectToAction("Login");
    }

    // --- LOGOUT SECTION ---
    public IActionResult Logout()
    {
        // Clears the session so the "UserId" becomes null
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}