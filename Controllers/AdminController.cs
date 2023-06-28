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

        public async Task<IActionResult> Admin()
        {
            List<UserViewModel> users = await _userService.GetUsers();
            ViewBag.Themes = await _userService.GetThemes();
            return View(users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddTheme(string themeName)
        {
            await _userService.AddTheme(themeName);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTheme(string themeName)
        {
            await _userService.DeleteTheme(themeName);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string[] ids)
        {
            await _userService.BlockUsers(ids, HttpContext.User);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GrantAdmin(string[] ids)
        {
            await _userService.GrantAdmin(ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAdmin(string[] ids)
        {
            await _userService.RevokeAdmin(ids, HttpContext.User);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(string[] ids)
        {
            await _userService.UnblockUsers(ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string[] ids)
        {
            await _userService.DeleteUsers(ids, HttpContext.User);
            return Ok();
        }
    }
}