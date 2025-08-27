using DALProject.Data;
using DALProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace DALProject.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly CarAppDbContext db;

        public DBInitializer(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CarAppDbContext db)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.db = db;
        }

        public async Task Initialize()
        {
            // migration if they are not applied
            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Initialization error: " + ex.Message);
            }


            // create roles if they are not applied

            if (!await roleManager.RoleExistsAsync(SD.CustomerRole))
            {
                // Create CurrentRole
                await roleManager.CreateAsync(new IdentityRole(SD.CustomerRole));
                await roleManager.CreateAsync(new IdentityRole(SD.TechnicianRole));
                await roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                await roleManager.CreateAsync(new IdentityRole(SD.DriverRole));

                // if roles are created , then we will create admin User as well
                var result = await userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "admin",
                    Street = "test 123 Ave",
                    City = "Chicago",
                    PhoneNumber = "1234567890",
                    Role = SD.AdminRole,
                }, "Admin123*");


                if (result.Succeeded)
                {
                    AppUser user = await userManager.FindByEmailAsync("admin@gmail.com");
                    await userManager.AddToRoleAsync(user, SD.AdminRole);
                }
            }

            return;
        }
    }
}
