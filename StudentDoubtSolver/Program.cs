using Microsoft.EntityFrameworkCore;
using StudentDoubtSolver.Models;

var builder = WebApplication.CreateBuilder(args);

// A. Add Services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(); // <--- ADD THIS

// B. Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed a development test user (only if not present)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!db.Users.Any(u => u.Email == "test@example.com"))
        {
            db.Users.Add(new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@1234",
                ScholarPoints = 0
            });
            db.SaveChanges();
        }
    }
    catch
    {
        // If seeding fails (e.g., DB not available), don't stop the app.
    }
}

// C. Use Services (ORDER MATTERS HERE)
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // <--- ADD THIS BEFORE AUTHENTICATION
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();