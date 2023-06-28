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

        public async Task<IActionResult> Index()
        {
            var latest_5_items = await _db.Items
                .AsNoTracking()
                .OrderByDescending(i => i.CreateDate)
                .Take(5)
                .Include(i => i.Collection)
                .Include(i => i.User)
                .ToListAsync();
            var biggest_5_collections = await _db.Collections
                .AsNoTracking()
                .OrderByDescending(c => c.Items.Count)
                .Take(5)
                .Include(c => c.User)
                .ToListAsync();
            var tags = await _db.Tags
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Latest_5_tems = latest_5_items;
            ViewBag.Biggest_5_collections = biggest_5_collections;
            ViewBag.Tags = tags;
            return View();
        }

        public async Task<IActionResult> Collection(string collectionId)
        {
            Collection collection = await _db.Collections
                .AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.Items)
                .FirstAsync(c => c.Id == collectionId);
            ViewBag.Collection = collection;
            return View();
        }

        public async Task<IActionResult> Item(string itemId)
        {
            Item item = await _db.Items
                .Include(i => i.User)
                .Include(i => i.Collection)
                .Include(i => i.Likes)
                .ThenInclude(l => l.User)
                .Include(i => i.Comments.OrderBy(c => c.Date))
                .ThenInclude(c => c.User)
                .FirstAsync(i => i.Id == itemId);
            Dictionary<string, string> customFields = GetCustomFields(item.Collection);
            ViewBag.Item = item;
            ViewBag.CustomFields = customFields;
            return View();
        }

        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return Ok();
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