using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers {
    public class SecretaryController : Controller 
    {
        private readonly UniversityAppDB _context;

        public SecretaryController(UniversityAppDB context)
        {
            _context = context;
        }

        private string GetDepartmentValue(Department department)
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

        public async Task<IActionResult> UsersList()
        {
            var users = await _context.Users
                .Include(u => u.Professor)
                .Include(u => u.Student)
                .Where(u => u.UserType != UserType.Secretary)
                .ToListAsync();

            return View(users);
        }


        // User Management Functions

        public IActionResult AddProfessor()
        {
            return View(new AddProfessorViewModel());
        }

        public IActionResult AddStudent()
        {
            return View(new AddStudentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfessor(AddProfessorViewModel model)
        {
           if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var exists = await _context.Users.AnyAsync( u => u.Username == model.Username );
            if (exists)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            try
            {
                // Insert the new professor user to the users table
                var userTypeName = UserType.Professor.ToString();
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO users (username, user_password, user_type) VALUES ({0}, {1}, {2}::user_type)",
                    model.Username, model.Password, userTypeName);

                // Get the new user
                var newUser = await _context.Users.FirstOrDefaultAsync( u => u.Username == model.Username );
                if (newUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add new professor.");
                    return View(model);
                }

                // Insert the new user to the professors table along with his additional details
                var departmentName = GetDepartmentValue(model.Department);
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO professors (user_id, afm, fullname, department) VALUES ({0}, {1}, {2}, {3}::department)",
                    newUser.Id, model.AFM, model.Fullname, departmentName);

                TempData["SuccessMessage"] = "New professor added successfully!";
                return RedirectToAction(nameof(AddProfessor));

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error while adding the professor: " + e.Message;
                return RedirectToAction(nameof(AddProfessor));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(AddStudentViewModel model)
        {
           if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var exists = await _context.Users.AnyAsync( u => u.Username == model.Username );
            if (exists)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            try
            {
                // Insert the new student user to the users table
                var userTypeName = UserType.Student.ToString();
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO users (username, user_password, user_type) VALUES ({0}, {1}, {2}::user_type)",
                    model.Username, model.Password, userTypeName);

                // Get the new user
                var newUser = await _context.Users.FirstOrDefaultAsync( u => u.Username == model.Username );
                if (newUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add new student.");
                    return View(model);
                }

                // Insert the new user to the students table along with his additional details
                var departmentName = GetDepartmentValue(model.Department);
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO students (user_id,fullname, department) VALUES ({0}, {1}, {2}::department)",
                    newUser.Id, model.Fullname, departmentName);

                TempData["SuccessMessage"] = "New student added successfully!";
                return RedirectToAction(nameof(AddStudent));

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error while adding the student: " + e.Message;
                return RedirectToAction(nameof(AddStudent));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId) {

            try
            {
                // Get the user if they exist
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(UsersList));
                }

                // Check their type and delete from the corresponding table first
                if (user.UserType == UserType.Professor)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM professors WHERE user_id = {0}", userId
                    );
                }
                else if (user.UserType == UserType.Student)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM students WHERE user_id = {0}", userId
                    );
                }

                // Delete from the users table as well
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM users WHERE user_id = {0}", userId
                );

                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction(nameof(UsersList));
            }
            catch (Exception e) 
            {
                TempData["ErrorMessage"] = "Error while deleting the user: " + e.Message;
                return RedirectToAction(nameof(UsersList));
            }
        }
    }
}