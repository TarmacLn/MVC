using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using System.Security.Claims;

namespace MVC.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {

        // University DB "Connection"
        private readonly UniversityAppDB _context;
        public StudentController(UniversityAppDB context)
        {
            _context = context;
        }

        private int? GetCurrentStudentId()
        {
            // if user is not authenticated, return null
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return null; 
            }

            // if student is authenticated, return their ID

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return userId;
            }
            return null;
        }
        


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GradesPerCourse()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                return RedirectToAction("Login", "Home");
            }
            
            var grades = await _context.CourseHasStudents
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Course.Title)
                .ToListAsync();

            return View(grades);
        }
        
        // View Grades per Semester
        public async Task<IActionResult> GradesPerSemester()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var grades = await _context.CourseHasStudents
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Course.Semester)
                .ThenBy(g => g.Course.Title)
                .ToListAsync();

            return View(grades);
        }

        public async Task<IActionResult> TotalAverage()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var grades = await _context.CourseHasStudents
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId && g.IsFinal == true && g.Grade.HasValue)
                .ToListAsync();

            return View(grades);
        }
    }

}