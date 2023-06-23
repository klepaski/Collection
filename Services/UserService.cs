using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using ToyCollection.Models;
using static Dropbox.Api.Files.ListRevisionsMode;

namespace ToyCollection.Services
{
    public interface IUserService
    {
        public Task<List<UserViewModel>> GetUsers();
        public Task<List<Theme>> GetThemes();
        public Task AddTheme(string name);
        public Task DeleteTheme(string name);
        public Task BlockUsers(string[] ids, ClaimsPrincipal userPrincipal);
        public Task GrantAdmin(string[] ids);
        public Task RevokeAdmin(string[] ids, ClaimsPrincipal userPrincipal);
        public Task UnblockUsers(string[] ids);
        public Task DeleteUsers(string[] ids, ClaimsPrincipal userPrincipal);
        public bool IsUserBlocked(string email);
        public void AddClaims(string username, string claimType, string claimValue);
        public void AddClaims(UserModel user, string claimType, string claimValue);
        public void RemoveClaims(UserModel user, string claimType, string claimValue);
        public Task LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(ILogger<UserService> logger, ApplicationDbContext db, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // THEMES----------------------------------------

        public async Task<List<Theme>> GetThemes()
        {
            return await _db.Themes.ToListAsync();
        }

        public async Task AddTheme(string name)
        {
            if (await _db.Themes.FindAsync(name) != null) return;
            await _db.Themes.AddAsync(new Theme() { Name = name });
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTheme(string name)
        {
            Theme? theme = await _db.Themes.FindAsync(name);
            if (theme == null) return;
            _db.Remove(theme);
            await _db.SaveChangesAsync();
        }

        // USERS---------------------------------------------

        public async Task<List<UserViewModel>> GetUsers()
        {
            var users = from user in _db.Users
                        join userRole in _db.UserRoles on user.Id equals userRole.UserId
                        join role in _db.Roles on userRole.RoleId equals role.Id
                        group role.Name by user.Id into rolesGroup
                        select new UserViewModel
                        {
                            Id = rolesGroup.Key,
                            UserName = _db.Users.FirstOrDefault(u => u.Id == rolesGroup.Key).UserName,
                            Email = _db.Users.FirstOrDefault(u => u.Id == rolesGroup.Key).Email,
                            IsBlocked = _db.Users.FirstOrDefault(u => u.Id == rolesGroup.Key).isBlocked,
                            Roles = rolesGroup.ToList()
                        };
            return await users.ToListAsync();
        }

        public async Task GrantAdmin(string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!userRoles.Contains("Admin")) await _userManager.AddToRoleAsync(user, "Admin");
            }
            await _db.SaveChangesAsync();
        }

        public async Task RevokeAdmin(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin")) await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            await _db.SaveChangesAsync();
        }

        public async Task BlockUsers(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel? user = await _db.Users.FirstAsync(x => x.Id.Equals(id));
                if (user == null) continue;
                user.isBlocked = true;
                await LogoutInactiveUserIfHeOnline(user.UserName, userPrincipal);
            }
            await _db.SaveChangesAsync();
        }

        public async Task UnblockUsers(string[] ids)
        {
            foreach (var id in ids)
            {
                UserModel? user = await _db.Users.FirstAsync(x => x.Id.Equals(id));
                if (user == null) continue;
                user.isBlocked = false;
            }
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUsers(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel? user = _userManager.FindByIdAsync(id).Result;
                if (user == null) continue;
                _db.Remove(user);
                await LogoutInactiveUserIfHeOnline(user.UserName, userPrincipal);
            }
            await _db.SaveChangesAsync();
        }

        public bool IsUserBlocked(string username)
        {
            UserModel? user = _userManager.FindByNameAsync(username).Result;
            return (user != null ? user.isBlocked : false);
        }

        public void AddClaims(string username, string claimType, string claimValue)
        {
            UserModel? user = _userManager.FindByNameAsync(username).Result;
            if (user == null) return;
            _userManager.AddClaimAsync(user, new Claim(claimType, claimValue)).Wait();
        }

        public async void AddClaims(UserModel user, string claimType, string claimValue)
        {
            await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
        }

        public void RemoveClaims(UserModel user, string claimType, string claimValue)
        {
            _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
        }

        public async Task LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal.Identity != null && userPrincipal.Identity.IsAuthenticated && username.Equals(userPrincipal.Identity.Name))
            {
                await _signInManager.SignOutAsync();
            }
        }
    }
}
