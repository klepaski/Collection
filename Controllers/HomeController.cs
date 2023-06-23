using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToyCollection.Models;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;

namespace ToyCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        private Dictionary<string, string> GetCustomFields(Collection collection)
        {
            string[] keys = new string[] {"CustomString1", "CustomString2", "CustomString3",
                 "CustomInt1", "CustomInt2", "CustomInt3", "CustomText1", "CustomText2", "CustomText3",
                 "CustomBool1", "CustomBool2", "CustomBool3", "CustomDate1", "CustomDate2", "CustomDate3" };
            Dictionary<string, string> result = new();
            foreach (string key in keys)
            {
                if (collection[key] != "" && collection[key] != null)
                {
                    result.Add(key, collection[key].ToString());
                }
            }
            return result;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Item(string itemId)
        {
            Item item = await _db.Items.FindAsync(itemId);
            Collection collection = await _db.Collections.FindAsync(item.CollectionId);
            Console.WriteLine(collection.Name);
            List<Comment> comments = await _db.Comments.Where(c => c.ItemId.Equals(itemId)).Include(c => c.User).OrderBy(c => c.Date).ToListAsync();
            List<Like> likes = await _db.Likes.Where(l => l.ItemId.Equals(itemId)).Include(l => l.User).ToListAsync();
            Dictionary<string, string> customFields = GetCustomFields(collection);
            ViewBag.Item = item;
            ViewBag.Collection = collection;
            ViewBag.Comments = comments;
            ViewBag.Likes = likes;
            ViewBag.CustomFields = customFields;
            return View();
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}