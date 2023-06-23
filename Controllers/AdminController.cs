using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToyCollection.Models;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using ToyCollection.Services;
using Microsoft.AspNetCore.Authorization;

namespace ToyCollection.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;

        public AdminController(ILogger<AdminController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            List<UserViewModel> users = await _userService.GetUsers();
            ViewBag.Themes = await _userService.GetThemes();
            return View(users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddTheme()
        {
            var theme = HttpContext.Request.Form["theme"];
            await _userService.AddTheme(theme);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTheme()
        {
            var name = HttpContext.Request.Form["themeName"];
            await _userService.DeleteTheme(name);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            await _userService.BlockUsers(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> GrantAdmin()
        {
            var ids = HttpContext.Request.Form["userId"];
            await _userService.GrantAdmin(ids);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAdmin()
        {
            var ids = HttpContext.Request.Form["userId"];
            await _userService.RevokeAdmin(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            await _userService.UnblockUsers(ids);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            await _userService.DeleteUsers(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }
    }
}