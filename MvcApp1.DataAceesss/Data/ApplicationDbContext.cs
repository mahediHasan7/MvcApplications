using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcApp1.Models;

namespace MvcApp1.DataAccess.Data;
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    // Entities
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }

    // constructor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // Seed data to the database
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // HasData adds seed data to the Entity type
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "SciFi", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Horror", DisplayOrder = 2 },
            new Category { Id = 3, Name = "Fiction", DisplayOrder = 3 },
            new Category { Id = 4, Name = "Drama", DisplayOrder = 4 }
            );

        // seed company data to bd
        modelBuilder.Entity<Company>().HasData(
            new Company { PostalCode = "12345", Name = "Company 1", City = "City 1", State = "State 1", StreetAddress = "1234 Street 1", PhoneNumber = "123-456-7890", Id = 1 },
            new Company { PostalCode = "54321", Name = "Company 2", City = "City 2", State = "State 2", StreetAddress = "4321 Street 2", PhoneNumber = "098-765-4321", Id = 2 },
            new Company { PostalCode = "67890", Name = "Company 3", City = "City 3", State = "State 3", StreetAddress = "6789 Street 3", PhoneNumber = "678-901-2345", Id = 3 },
            new Company { PostalCode = "09876", Name = "Company 4", City = "City 4", State = "State 4", StreetAddress = "0987 Street 4", PhoneNumber = "098-765-4321", Id = 4 }
            );

        // seed product data to db
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Title = "Ice Caps",
                Author = "Billy Spark",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "SWD9999001",
                ListPrice = 99,
                Price = 90,
                Price50 = 85,
                Price100 = 80,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product
            {
                Id = 2,
                Title = "Spark",
                Author = "Nancy Hoover",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "CAW777777701",
                ListPrice = 40,
                Price = 30,
                Price50 = 25,
                Price100 = 20,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product
            {
                Id = 3,
                Title = "Behind The Lens",
                Author = "Julian Button",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "RITO5555501",
                ListPrice = 55,
                Price = 50,
                Price50 = 40,
                Price100 = 35,
                CategoryId = 3,
                ImageUrl = ""
            },
            new Product
            {
                Id = 4,
                Title = "Adenturous Eating",
                Author = "Abby Muscles",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "WS3333333301",
                ListPrice = 70,
                Price = 65,
                Price50 = 60,
                Price100 = 55,
                CategoryId = 4,
                ImageUrl = ""
            },
            new Product
            {
                Id = 5,
                Title = "Love, Elliot",
                Author = "Ron Parker",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "SOTJ1111111101",
                ListPrice = 30,
                Price = 27,
                Price50 = 25,
                Price100 = 20,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product
            {
                Id = 6,
                Title = "Culitvate Workplace",
                Author = "Laura Phantom",
                Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "FOT000000001",
                ListPrice = 25,
                Price = 23,
                Price50 = 22,
                Price100 = 20,
                CategoryId = 2,
                ImageUrl = ""
            }
            );
    }



}
