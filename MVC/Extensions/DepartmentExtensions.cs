using MVC.Models;

namespace MVC.Extensions
{
    public static class DepartmentExtensions
    {
        public static string ToDbValue(this Department department)
        {
            return department switch
            {
                Department.ComputerScience => "Computer Science",
                Department.BusinessAdministration => "Business Administration",
                Department.Sociology => "Sociology",
                _ => "Computer Science"
            };
        }

        public static Department FromDbValue(string dbValue)
        {
            return dbValue switch
            {
                "Computer Science" => Department.ComputerScience,
                "Business Administration" => Department.BusinessAdministration,
                "Sociology" => Department.Sociology,
                _ => Department.ComputerScience
            };
        }
    }
}
