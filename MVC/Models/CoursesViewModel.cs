namespace MVC.Models
{
    public class CoursesViewModel
    {
        // public IEnumerable<Course> AllCourses { get; set; } = null!;
        public IEnumerable<Course> ComputerScienceCourses { get; set; } = new List<Course>();
        public IEnumerable<Course> BusinessAdministrationCourses { get; set; } = new List<Course>();
        public IEnumerable<Course> SociologyCourses { get; set; } = new List<Course>();
        public IEnumerable<Course> UnassignedCourses { get; set; } = new List<Course>();
    }
}