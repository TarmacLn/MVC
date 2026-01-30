using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityAppDB _context;
        public HomeController(UniversityAppDB context)
        {
            _context = context;
        }

        private string GetDepartmentDbValue(Department? department)
        {
            return department switch
            {
                Department.ComputerScience => "Computer Science",
                Department.BusinessAdministration => "Business Administration",
                Department.Sociology => "Sociology",
                _ => "Computer Science"
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return user.UserType switch
            {
                UserType.Secretary => RedirectToAction("Index", "Secretary"),
                UserType.Professor => RedirectToAction("Index", "Professor"),
                UserType.Student => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var exists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (exists)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            var userTypeName = model.UserType.ToString();
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO users (username, user_password, user_type) VALUES ({0}, {1}, {2}::user_type)",
                model.Username, model.Password, userTypeName);

            var newUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (newUser == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create user.");
                return View(model);
            }

            // Insert into respective table based on user type
            var tableName = model.UserType switch
            {
                UserType.Student => "students",
                UserType.Professor => "professors",
                UserType.Secretary => "secretaries",
                _ => throw new InvalidOperationException($"Unknown user type: {model.UserType}")
            };

            if (model.UserType == UserType.Student)
            {
                var departmentName = GetDepartmentDbValue(model.Department);
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO students (user_id, fullname, department) VALUES ({0}, {1}, {2}::department)",
                    newUser.Id, model.Fullname, departmentName);
            }
            else if (model.UserType == UserType.Professor)
            {
                var departmentName = GetDepartmentDbValue(model.Department);
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO professors (user_id, afm, fullname, department) VALUES ({0}, {1}, {2}, {3}::department)",
                    newUser.Id, model.AFM, model.Fullname, departmentName);
            }
            else if (model.UserType == UserType.Secretary)
            {
                var departmentName = GetDepartmentDbValue(model.Department);
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO secretaries (user_id, fullname, phonenumber, department) VALUES ({0}, {1}, {2}, {3}::department)",
                    newUser.Id, model.Fullname, model.PhoneNumber, departmentName);
            }

            // Auto-login after successful sign up
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Username),
                new Claim(ClaimTypes.Role, newUser.UserType.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Users");
        }
    }
}
