using Microsoft.EntityFrameworkCore;
using MvcApp1.Models;

namespace MvcApp1.Data;
public class ApplicationDbContext : DbContext
{
    // Entities
    public DbSet<Category> Categories { get; set; }

    // constructor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // Seed data to the database
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // HasData adds seed data to the Entity type
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Web Development", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Mobile Development", DisplayOrder = 2 },
            new Category { Id = 3, Name = "Game Development", DisplayOrder = 3 },
            new Category { Id = 4, Name = "Machine Learning", DisplayOrder = 4 },
            new Category { Id = 5, Name = "Artificial Intelligence", DisplayOrder = 5 },
            new Category { Id = 6, Name = "Data Science", DisplayOrder = 6 },
            new Category { Id = 7, Name = "Cloud Computing", DisplayOrder = 7 },
            new Category { Id = 8, Name = "Cyber Security", DisplayOrder = 8 }
            );
    }
}
