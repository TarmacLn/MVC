using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using System.Security.Claims;

namespace MVC.Controllers
{
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
        
        // View Grades per Subject
        public async Task<IActionResult> GradesPerSubject()
        {
            var studentId = GetCurrentStudentId();
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
            var grades = await _context.CourseHasStudents
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Course.Semester)
                .ToListAsync();

            return View(grades);
        }

        // View All Passed Courses
        public async Task<IActionResult> PassedCourses()
        {
            var studentId = GetCurrentStudentId();
            var grades = await _context.CourseHasStudents
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId && g.Grade >= 5)
                .ToListAsync();

            return View(grades);
        }
    }

}