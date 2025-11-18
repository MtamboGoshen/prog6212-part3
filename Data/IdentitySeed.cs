using ContractMonthlyClaim.Models;
using Microsoft.AspNetCore.Identity;

namespace ContractMonthlyClaim.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles
            await SeedRoleAsync(roleManager, "HR"); // <-- NEW HR ROLE
            await SeedRoleAsync(roleManager, "Manager");
            await SeedRoleAsync(roleManager, "Programme Coordinator");
            await SeedRoleAsync(roleManager, "Lecturer");

            // Seed Users
            // NEW HR User
            await SeedUserAsync(userManager, "hr@test.com", "Password123", "HR", "Admin", "User", 0);

            // Updated test users with new properties
            await SeedUserAsync(userManager, "manager@test.com", "Password123", "Manager", "Manager", "User", 0);
            await SeedUserAsync(userManager, "coordinator@test.com", "Password123", "Programme Coordinator", "Coord", "User", 0);
            await SeedUserAsync(userManager, "lecturer@test.com", "Password123", "Lecturer", "Lecturer", "User", 500); // Example rate
        }

        private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // MODIFIED: This method now accepts first name, last name, and hourly rate
        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string userName, string password, string role, string firstName, string lastName, decimal hourlyRate)
        {
            if (await userManager.FindByNameAsync(userName) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true,
                    FirstName = firstName,      // <-- NEW
                    LastName = lastName,        // <-- NEW
                    HourlyRate = hourlyRate     // <-- NEW
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}