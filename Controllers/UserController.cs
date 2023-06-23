using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToyCollection.Models;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using ToyCollection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Dropbox.Api.Files;
using Dropbox.Api;
using static Dropbox.Api.Files.SearchMatchType;
using static Dropbox.Api.Files.ListRevisionsMode;
using System.Globalization;

namespace ToyCollection.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IDropboxService _dropboxService;

        public UserController(ILogger<UserController> logger, ApplicationDbContext db, IDropboxService dropboxService)
        {
            _logger = logger;
            _db = db;
            _dropboxService = dropboxService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Themes = _db.Themes.ToList();
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.Find(userId);
            List<Collection> collections = _db.Collections.Where(x => x.User == user).ToList();
            return View(collections);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollection()
        {
            var form = HttpContext.Request.Form;
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.Users.FindAsync(userId);
            Collection? collection = new Collection()
            {
                User = user,
                Name = form["Name"],
                Description = form["Description"],
                Theme = form["Theme"],
                CustomString1 = form["CustomString1"],
                CustomString2 = form["CustomString2"],
                CustomString3 = form["CustomString3"],
                CustomInt1 = form["CustomInt1"],
                CustomInt2 = form["CustomInt2"],
                CustomInt3 = form["CustomInt3"],
                CustomText1 = form["CustomText1"],
                CustomText2 = form["CustomText2"],
                CustomText3 = form["CustomText3"],
                CustomBool1 = form["CustomBool1"],
                CustomBool2 = form["CustomBool2"],
                CustomBool3 = form["CustomBool3"],
                CustomDate1 = form["CustomDate1"],
                CustomDate2 = form["CustomDate2"],
                CustomDate3 = form["CustomDate3"]
            };
            await _db.Collections.AddAsync(collection);
            await _db.SaveChangesAsync();
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditCollection(string id, string name, string description, string theme,
            string customString1, string customString2, string customString3,
            string customInt1, string customInt2, string customInt3,
            string customText1, string customText2, string customText3,
            string customBool1, string customBool2, string customBool3,
            string customDate1, string customDate2, string customDate3)
        {
            Collection? collection = await _db.Collections.FindAsync(id);
            if (collection == null) return Ok();
            collection.Name = name;
            collection.Description = description;
            collection.Theme = theme;
            collection.CustomString1 = customString1;
            collection.CustomString2 = customString2;
            collection.CustomString3 = customString3;
            collection.CustomInt1 = customInt1;
            collection.CustomInt2 = customInt2;
            collection.CustomInt3 = customInt3;
            collection.CustomText1 = customText1;
            collection.CustomText2 = customText2;
            collection.CustomText3 = customText3;
            collection.CustomBool1 = customBool1;
            collection.CustomBool2 = customBool2;
            collection.CustomBool3 = customBool3;
            collection.CustomDate1 = customDate1;
            collection.CustomDate2 = customDate2;
            collection.CustomDate3 = customDate3;
            await _db.SaveChangesAsync();
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollection()
        {
            var id = HttpContext.Request.Form["collectionId"];
            Collection? collection = await _db.Collections.FindAsync(id);
            if (collection == null) return Ok();
            _db.Remove(collection);
            await _db.SaveChangesAsync();
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public async Task<string> UploadImage(string id)
        {
            IFormFile? file = Request.Form.Files[0];
            var result = await _dropboxService.UploadImage(id, file);
            return result;
        }

        //====================== I T E M S =================================

        private string[] keys = new string[] {"CustomString1", "CustomString2", "CustomString3",
                        "CustomInt1", "CustomInt2", "CustomInt3", "CustomText1", "CustomText2", "CustomText3",
                        "CustomBool1", "CustomBool2", "CustomBool3", "CustomDate1", "CustomDate2", "CustomDate3" };


        //private string GetCollectionOfItem(Item? item)
        //{
        //    Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.Equals(item.CollectionId));
        //    if (collection == null) return "";
        //    return collection.Id.ToString();
        //}

        private Dictionary<string, string> GetCustomFields(string id)
        {
            Dictionary<string, string> result = new();
            Collection? collection = _db.Collections.Find(id);
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

        public async Task<IActionResult> Items(string collectionId)
        {
            ViewBag.CollectionId = collectionId;
            ViewBag.CollectionName = (await _db.Collections.FindAsync(collectionId)).Name;
            ViewBag.Items = (_db.Items.Where(x => x.CollectionId.Equals(collectionId))).ToList();
            ViewBag.CustomFields = GetCustomFields(collectionId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem()
        {
            Dictionary<string, string> parms = new();
            var form = HttpContext.Request.Form;
            var collectionId = form["collectionId"];
            var customFields = GetCustomFields(collectionId);
            var name = form["Name"];

            foreach (var field in customFields)
            {
                parms.Add(field.Key, form[field.Key]);
            }

            Item item = new Item();
            item.Name = name;
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            item.User = await _db.Users.FindAsync(userId);
            item.Collection = await _db.Collections.FindAsync(collectionId);

            foreach (var parm in parms)
            {
                item[parm.Key] = parm.Key switch
                {
                    string a when a.Contains("Int") => Int32.Parse(parm.Value),
                    string b when b.Contains("Bool") => (parm.Value == "on") ? true : false,
                    string c when c.Contains("Date") => DateTime.Parse(parm.Value),
                    _ => parm.Value
                };
            }
            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();
            return RedirectPermanent($"~/User/Items?collectionId={collectionId}");
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(string id, string name, string tags,
            string? customString1, string? customString2, string? customString3,
            string? customInt1, string? customInt2, string? customInt3,
            string? customText1, string? customText2, string? customText3,
            string? customBool1, string? customBool2, string? customBool3,
            string? customDate1, string? customDate2, string? customDate3)
        {
            Item? item = await _db.Items.FindAsync(id);
            if (item == null) return Ok();
            item.Name = name;
            if (customString1 != null) item.CustomString1 = customString1;
            if (customString2 != null) item.CustomString2 = customString2;
            if (customString3 != null) item.CustomString3 = customString3;
            if (customInt1 != null) item.CustomInt1 = Int32.Parse(customInt1);
            if (customInt2 != null) item.CustomInt2 = Int32.Parse(customInt2);
            if (customInt3 != null) item.CustomInt3 = Int32.Parse(customInt3);
            if (customText1 != null) item.CustomText1 = customText1;
            if (customText2 != null) item.CustomText2 = customText2;
            if (customText3 != null) item.CustomText3 = customText3;
            if (customBool1 != null) item.CustomBool1 = (customBool1 == "on") ? true : false;
            if (customBool2 != null) item.CustomBool2 = (customBool2 == "on") ? true : false;
            if (customBool3 != null) item.CustomBool3 = (customBool3 == "on") ? true : false;
            if (customDate1 != null) item.CustomDate1 = DateTime.Parse(customDate1);
            if (customDate2 != null) item.CustomDate2 = DateTime.Parse(customDate2);
            if (customDate3 != null) item.CustomDate3 = DateTime.Parse(customDate3);
            await _db.SaveChangesAsync();
            return RedirectPermanent($"~/User/Items?collectionId={item.CollectionId}");
        }
        

        [HttpPost]
        public async Task<IActionResult> DeleteItem()
        {
            var itemId = HttpContext.Request.Form["itemId"];
            Item? item = await _db.Items.FindAsync(itemId);
            if (item == null) return Ok();
            _db.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectPermanent($"~/User/Items?collectionId={item.CollectionId}");
        }

        //==========================================
        [HttpPost]
        public async Task<IActionResult> CreateComment(string? itemId, string? userId, string text, string date)
        {
            Item item = await _db.Items.FindAsync(itemId);
            UserModel user = await _db.Users.FindAsync(userId);
            Comment newComment = new Comment()
            {
                Item = item,
                User = user,
                Text = text,
                Date = DateTime.ParseExact(date, "dd.MM.yyyy, HH:mm:ss", CultureInfo.InvariantCulture)
            };
            await _db.Comments.AddAsync(newComment);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LikeItem(string? itemId, string? userId)
        {
            if (await _db.Likes.FindAsync(itemId, userId) != null) return Ok();    //complicated key!!
            Like newLike = new Like() { ItemId = itemId, UserId = userId };
            await _db.Likes.AddAsync(newLike);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DislikeItem(string? itemId, string? userId)
        {
            Like? likeToDelete = await _db.Likes.FindAsync(itemId, userId);    //complicated key!!
            if (likeToDelete == null) return Ok();
            _db.Remove(likeToDelete);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}