using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        private static readonly Dictionary<int, string> CourseDepartmentMapping = new()
        {
            // Computer Science
            { 1, "Computer Science" },
            { 2, "Computer Science" },
            { 3, "Computer Science" },
            { 4, "Computer Science" },
            { 5, "Computer Science" },
            { 6, "Computer Science" },
            {7, "Computer Science" },
            {8, "Computer Science" },

            // Sociology
            { 9, "Sociology" },
            { 10, "Sociology" },
            { 11, "Sociology" },
            { 12, "Sociology" },
            { 13, "Sociology" },
            { 14, "Sociology" },
            {15, "Sociology" },
            {16, "Sociology" },

            // Business Administration
            { 17, "Business Administration" },
            { 18, "Business Administration" },
            { 19, "Business Administration" },
            { 20, "Business Administration" },
            { 21, "Business Administration" },
            { 22, "Business Administration" },
            {23, "Business Administration" },
            {24, "Business Administration" },
        };

        private string GetDepartmentCourseName(int courseId)
        {
            return CourseDepartmentMapping.TryGetValue(courseId, out var dept) ? dept : "Unknown";
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

        public async Task<IActionResult> CoursesList()
        {
            var courses = await _context.Courses
                .Include(c => c.Professor)
                .ToListAsync();

            return View(courses);          
        }

        public async Task<IActionResult> StudentsCoursesList()
        {
            var students = await _context.Students
                .Include(ec => ec.EnrolledCourses)
                    .ThenInclude(c => c.Course)
                .ToListAsync();

                return View(students);
        }


        [HttpGet]
        public async Task<IActionResult> EnrollInCourse(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync( s => s.UserId ==studentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Error occured while selecting this student";
                return RedirectToAction(nameof(UsersList));
            }

            var allCourses = await _context.Courses.ToListAsync();  

            var filteredCourses = allCourses
                .Where(c => GetDepartmentCourseName(c.CourseId) == student.Department)
                .ToList();

            var model = new EnrollInCourseViewModel
            {
                StudentId = studentId,
                StudentName = student.Fullname,
                StudentDepartment = student.Department,
                Courses= new SelectList(filteredCourses, "CourseId", "Title")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollInCourse(EnrollInCourseViewModel model)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == model.StudentId);

            if (!ModelState.IsValid)
            {
                var allCourses = await _context.Courses.ToListAsync();

                var filteredCourses = allCourses
                    .Where(c=> GetDepartmentCourseName(c.CourseId) == student!.Department)
                    .ToList();
                
                model.StudentName = student?.Fullname ?? "";
                model.StudentDepartment = student?.Department ?? "";
                model.Courses = new SelectList(filteredCourses, "CourseId", "Title");

                return View(model);
            }

            // Some validation
            var course = await _context.Courses.FindAsync(model.CourseId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Error occured while enrolling in this course";
                return RedirectToAction(nameof(EnrollInCourse), new { studentId = model.StudentId });
            }

            var alreadyEnrolled = await _context.CourseHasStudents
                .AnyAsync(chs => chs.StudentId == model.StudentId && chs.CourseId == model.CourseId);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "Student is already enrolled in this course.";
                return RedirectToAction(nameof(EnrollInCourse), new { studentId = model.StudentId });
            }

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO course_has_student (course_id, student_id) VALUES ({0}, {1})",
                    model.CourseId, model.StudentId);
                
                TempData["SuccessMessage"] = "Student enrolled in course successfully!";
                return RedirectToAction(nameof(UsersList));
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error while enrolling student: " + e.Message;
                return RedirectToAction(nameof(EnrollInCourse), new { studentId = model.StudentId });
            }

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


        // Course Management Functions

        [HttpGet]
        public async Task<IActionResult> AssignCourse(int professorId)
        {
            var professors = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == professorId);

            if (professors == null)
            {
                TempData["ErrorMessage"] = "Error occured while selecting this professor";
                return RedirectToAction(nameof(UsersList));
            }

            var allCourses = await _context.Courses.ToListAsync(); 

            var filteredCourses = allCourses
                .Where(c => GetDepartmentCourseName(c.CourseId) == professors.Department)
                .ToList();
            
            var model = new AssignCourseViewModel
            {
                ProfessorId = professorId,
                ProfessorName = professors.Fullname,
                ProfessorDepartment = professors.Department,
                Courses= new SelectList(filteredCourses, "CourseId", "Title")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCourse(AssignCourseViewModel model)
        {
            var professor = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == model.ProfessorId);

            if (!ModelState.IsValid)
            {
                var allCourses = await _context.Courses.ToListAsync();
                
                var filteredCourses = allCourses
                    .Where(c=> GetDepartmentCourseName(c.CourseId) == professor!.Department)
                    .ToList();
                
                model.ProfessorName = professor?.Fullname ?? "";
                model.ProfessorDepartment = professor?.Department ?? "";
                model.Courses = new SelectList(filteredCourses, "CourseId", "Title");

                return View(model);
            }

            var course = await _context.Courses.FindAsync(model.CourseId);
            
            if (course == null)
            {
                TempData["ErrorMessage"] = "Error occured while assigning the course";
                return RedirectToAction(nameof(UsersList));
            }

            if (course.ProfessorId == model.ProfessorId)
            {
                TempData["ErrorMessage"] = "This course is already assigned to this professor";
                return RedirectToAction(nameof(AssignCourse), new { professorId = model.ProfessorId });
            }

            if (course.ProfessorId != null)
            {
                TempData["ErrorMessage"] = "This course is already assigned to another professor";
                return RedirectToAction(nameof(AssignCourse), new { professorId = model.ProfessorId });
            }

            // Reassign course to the selected professor
            course.ProfessorId = model.ProfessorId;
            await _context.SaveChangesAsync();
        
            TempData["SuccessMessage"] = "Course assigned to professor successfully!";
            return RedirectToAction(nameof(UsersList));
        }
    }

}