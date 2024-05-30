using Microsoft.AspNetCore.Identity;
using ProductManagementAPP.Models;

namespace ProductManagementAPP.SeedData
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            string userEmail = "user@example.com";
            string userPassword = "SecurePassword123!";

            // Ensure the SuperAdmin role
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Ensure the SuperAdmin user
            var User = await userManager.FindByEmailAsync(userEmail);
            if (User == null)
            {
                User = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "User",
                    LastName = "User Last Name",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,

                };

                var result = await userManager.CreateAsync(User, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(User, "User");

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }

        }
    }

}
