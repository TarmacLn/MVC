using System.Data;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UniversityAppDB _context;
        public UsersController(UniversityAppDB context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string userType)
        {
            if (ModelState.IsValid)
            {
                var userTypeEnum = (UserType)int.Parse(userType);
                var userTypeName = userTypeEnum.ToString();
                
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO users (username, user_password, user_type) VALUES ({0}, {1}, {2}::user_type)",
                    username, password, userTypeName);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}