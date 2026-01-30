using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using System.Security.Claims;

namespace MVC.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {
        private readonly UniversityAppDB _context;

        public ProfessorController(UniversityAppDB context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CoursesList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Home");
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userIdInt);

            if (professor == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var courses = await _context.Courses
                .Where(c => c.ProfessorId == professor.UserId)
                .Include(c => c.CourseHasStudents)
                    .ThenInclude(chs => chs.Student)
                .ToListAsync();

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> AddGrade(int courseId, int studentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Home");
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userIdInt);

            if (professor == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.ProfessorId == professor.UserId);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found or you don't have permission to grade this course.";
                return RedirectToAction(nameof(CoursesList));
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == studentId);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToAction(nameof(CoursesList));
            }

            var isEnrolled = await _context.CourseHasStudents
                .AnyAsync(chs => chs.CourseId == courseId && chs.StudentId == studentId);

            if (!isEnrolled)
            {
                TempData["ErrorMessage"] = "Student is not enrolled in this course.";
                return RedirectToAction(nameof(CoursesList));
            }

            var hasFinalGrade = await _context.CourseHasStudents
                .AnyAsync(chs => chs.CourseId == courseId && chs.StudentId == studentId && chs.IsFinal == true);

            if (hasFinalGrade)
            {
                TempData["ErrorMessage"] = "Student already has a final grade for this course.";
                return RedirectToAction(nameof(CoursesList));
            }

            var model = new AddGradeViewModel
            {
                CourseId = courseId,
                StudentId = studentId,
                StudentName = student.Fullname,
                CourseName = course.Title
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(AddGradeViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Home");
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userIdInt);

            if (professor == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                var course = await _context.Courses.FindAsync(model.CourseId);
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == model.StudentId);
                model.CourseName = course?.Title ?? "";
                model.StudentName = student?.Fullname ?? "";
                return View(model);
            }

            var professorCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == model.CourseId && c.ProfessorId == professor.UserId);

            if (professorCourse == null)
            {
                TempData["ErrorMessage"] = "You don't have permission to grade this course.";
                return RedirectToAction(nameof(CoursesList));
            }

            var hasFinalGrade = await _context.CourseHasStudents
                .AnyAsync(chs => chs.CourseId == model.CourseId && chs.StudentId == model.StudentId && chs.IsFinal == true);

            if (hasFinalGrade)
            {
                TempData["ErrorMessage"] = "Student already has a final grade for this course.";
                return RedirectToAction(nameof(CoursesList));
            }

            try
            {
                var newGrade = new CourseHasStudent
                {
                    CourseId = model.CourseId,
                    StudentId = model.StudentId,
                    Grade = model.Grade,
                    Description = model.Description,
                    IsFinal = model.IsFinal
                };

                _context.CourseHasStudents.Add(newGrade);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Grade added successfully!";
                return RedirectToAction(nameof(CoursesList));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error while adding grade: " + ex.Message;
                return RedirectToAction(nameof(AddGrade), new { courseId = model.CourseId, studentId = model.StudentId });
            }
        }
    }
}
