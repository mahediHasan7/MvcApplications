using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MahediBookStore.DataAccess.Data;
using MahediBookStore.Models;
using MahediBookStore.Utility;

namespace MahediBookStore.DataAccess.DbInitializer
{
    public class DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : IDbInitializer
    {
        private readonly ApplicationDbContext _context = context;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public void Initialize()
        {
            // Migrate if there is any pending migration left
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }


            // Create Roles if any of them not exists and the Admin user and then assign Admin role to the Admin user
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                //  Create a new admin Application User
                _userManager.CreateAsync(
                    new ApplicationUser
                    {
                        Name = "Admin",
                        UserName = "bookstoreadmin@gmail.com",
                        Email = "bookstoreadmin@gmail.com",
                        PhoneNumber = "1234567890",
                        StreetAddress = "123 Main St",
                        State = "CA",
                        City = "Los Angeles",
                        PostalCode = "90001"
                    }, "Password@123").GetAwaiter().GetResult();

                // Retrieving the newly created Admin user and assigning the admin role to it
                ApplicationUser admin = _context.ApplicationUsers.FirstOrDefault(user => user.Email == "bookstoreadmin@gmail.com");
                _userManager.AddToRoleAsync(admin, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
