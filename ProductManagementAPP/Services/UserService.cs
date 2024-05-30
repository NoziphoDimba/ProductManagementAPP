using Microsoft.AspNetCore.Identity;
using ProductManagementAPP.Models;

namespace ProductManagementAPP.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);

        public class UserService : IUserService
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public UserService(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<ApplicationUser> GetUserByIdAsync(string userId)
            {
                return await _userManager.FindByIdAsync(userId);
            }
        }
    }
}
