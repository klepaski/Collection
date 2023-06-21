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
        public List<UserViewModel> GetUsers();
        public List<Theme> GetThemes();
        public void AddTheme(string name);
        public void DeleteTheme(string name);
        public void BlockUsers(string[] ids, ClaimsPrincipal userPrincipal);
        public void GrantAdmin(string[] ids);
        public void RevokeAdmin(string[] ids, ClaimsPrincipal userPrincipal);
        public void UnblockUsers(string[] ids);
        public void DeleteUsers(string[] ids, ClaimsPrincipal userPrincipal);
        public bool IsUserBlocked(string email);
        public void AddClaims(string username, string claimType, string claimValue);
        public void AddClaims(UserModel user, string claimType, string claimValue);
        public void RemoveClaims(UserModel user, string claimType, string claimValue);
        public void LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal);
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

        public List<Theme> GetThemes()
        {
            return _db.Themes.ToList();
        }

        public void AddTheme(string name)
        {
            Theme? theme = _db.Themes.FirstOrDefault(x => x.Name.Equals(name));
            if (theme != null) return;
            _db.Themes.Add(new Theme() { Name = name });
            _db.SaveChanges();
        }

        public void DeleteTheme(string name)
        {
            Theme? theme = _db.Themes.FirstOrDefault(x => x.Name.Equals(name));
            if (theme == null) return;
            _db.Remove(theme);
            _db.SaveChanges();
        }

        // USERS---------------------------------------------

        public List<UserViewModel> GetUsers()
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
            return users.ToList();
        }

        public void GrantAdmin(string[] ids)
        {
            foreach (var id in ids)
            {
                UserModel? user = _userManager.FindByIdAsync(id).Result;
                IdentityRole? role = _roleManager.FindByNameAsync("Admin").Result;
                if (_db.UserRoles.Find(user.Id, role.Id) != null) continue;
                IdentityUserRole<string> userRole = new();
                userRole.UserId = user.Id;
                userRole.RoleId = role.Id;
                _db.UserRoles.Add(userRole);
            }
            _db.SaveChanges();
        }

        public void RevokeAdmin(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel? user = _userManager.FindByIdAsync(id).Result;
                IdentityRole? role = _roleManager.FindByNameAsync("Admin").Result;
                IdentityUserRole<string>? userRole = _db.UserRoles.Find(user.Id, role.Id);
                if (userRole == null) continue;
                _db.Remove(userRole);
                LogoutInactiveUserIfHeOnline(user.UserName, userPrincipal);
            }
            _db.SaveChanges();
        }

        public void BlockUsers(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel? user = _db.Users.FirstOrDefault(x => x.Id.Equals(id));
                if (user == null) continue;
                user.isBlocked = true;
                LogoutInactiveUserIfHeOnline(user.UserName, userPrincipal);
            }
            _db.SaveChanges();
        }

        public void UnblockUsers(string[] ids)
        {
            foreach (var id in ids)
            {
                UserModel? user = _db.Users.FirstOrDefault(x => x.Id.Equals(id));
                if (user == null) continue;
                user.isBlocked = false;
            }
            _db.SaveChanges();
        }

        public void DeleteUsers(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel user = _userManager.FindByIdAsync(id).Result;
                _db.Remove(user);
                LogoutInactiveUserIfHeOnline(user.UserName, userPrincipal);
            }
            _db.SaveChanges();
        }

        public bool IsUserBlocked(string username)
        {
            UserModel user = _userManager.FindByNameAsync(username).Result;
            return (user != null ? user.isBlocked : false);
        }

        public void AddClaims(string username, string claimType, string claimValue)
        {
            UserModel user = _userManager.FindByNameAsync(username).Result;
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

        public void LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal.Identity != null && userPrincipal.Identity.IsAuthenticated && username.Equals(userPrincipal.Identity.Name))
            {
                _signInManager.SignOutAsync();
            }
        }
    }
}
