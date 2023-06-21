using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToyCollection.Models;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using ToyCollection.Services;
using ToyCollection.Models;
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

        public IActionResult Index()
        {
            List<UserViewModel> users = _userService.GetUsers();
            ViewBag.Themes = _userService.GetThemes();
            return View(users.ToList());
        }

        [HttpPost]
        public IActionResult AddTheme()
        {
            var theme = HttpContext.Request.Form["theme"];
            _userService.AddTheme(theme);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult DeleteTheme()
        {
            var name = HttpContext.Request.Form["themeName"];
            _userService.DeleteTheme(name);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult BlockUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            _userService.BlockUsers(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult GrantAdmin()
        {
            var ids = HttpContext.Request.Form["userId"];
            _userService.GrantAdmin(ids);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult RevokeAdmin()
        {
            var ids = HttpContext.Request.Form["userId"];
            _userService.RevokeAdmin(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult UnblockUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            _userService.UnblockUsers(ids);
            return RedirectPermanent("~/Admin/Index");
        }

        [HttpPost]
        public IActionResult DeleteUser()
        {
            var ids = HttpContext.Request.Form["userId"];
            _userService.DeleteUsers(ids, HttpContext.User);
            return RedirectPermanent("~/Admin/Index");
        }
    }
}