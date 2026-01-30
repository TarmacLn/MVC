using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Data;

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
    }
}
