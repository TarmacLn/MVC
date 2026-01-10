using Microsoft.EntityFrameworkCore;

public class UniversityAppDB : DbContext
{
    public UniversityAppDB(DbContextOptions<UniversityAppDB> options)
        : base(options)
    {
        
    }
}

