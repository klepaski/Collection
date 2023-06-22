using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToyCollection.Models;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

        private Dictionary<string, string> GetCustomFields(string id)
        {
            string[] keys = new string[] {"CustomString1", "CustomString2", "CustomString3",
                        "CustomInt1", "CustomInt2", "CustomInt3", "CustomText1", "CustomText2", "CustomText3",
                        "CustomBool1", "CustomBool2", "CustomBool3", "CustomDate1", "CustomDate2", "CustomDate3" };
            Dictionary<string, string> result = new();
            Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (collection == null) return result;
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

        public IActionResult Item(string itemId)
        {
            string collectionId = _db.Items.First(i => i.Id.ToString() == itemId).CollectionId.ToString();
            ViewBag.Item = _db.Items.First(i => i.Id.ToString() == itemId);
            ViewBag.Collection = _db.Collections.First(c => c.Id.ToString() == collectionId);
            ViewBag.Comments = _db.Comments.Where(c => c.ItemId.ToString() == itemId).Include(c => c.User).ToList();
            ViewBag.Likes = _db.Likes.Where(l => l.ItemId.ToString() == itemId).Include(l => l.User).ToList();
            ViewBag.CustomFields = GetCustomFields(collectionId);
            //ViewBag.CurrentUser = HttpContext.User;
            return View();
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