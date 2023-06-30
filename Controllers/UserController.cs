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
//using Azure.Core;
//using Dropbox.Api.Files;
//using Dropbox.Api;
//using static Dropbox.Api.Files.SearchMatchType;
//using static Dropbox.Api.Files.ListRevisionsMode;
using System.Globalization;
using NuGet.Protocol.Core.Types;
using Dropbox.Api.Files;
using Tag = ToyCollection.Models.Tag;

namespace ToyCollection.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IDropboxService _dropboxService;

        public UserController(ILogger<UserController> logger, ApplicationDbContext db, IDropboxService dropboxService, IUserService userService)
        {
            _logger = logger;
            _db = db;
            _dropboxService = dropboxService;
            _userService = userService;
        }

        public async Task<IActionResult> Collections(string userId)
        {
            ViewBag.Themes = await _userService.GetThemes();
            ViewBag.User = await _db.Users.FindAsync(userId);
            ViewBag.Collections = await _db.Collections
                .Include(c => c.User)
                .Where(c => c.User.Id == userId)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollection()
        {
            var form = HttpContext.Request.Form;
            Collection? collection = new Collection()
            {
                UserId = form["UserId"],
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
            return RedirectPermanent("~/User/Collections?userId=" + form["UserId"]);
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
            if (collection == null) return NotFound();
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
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollection(string collectionId)
        {
            Collection? collection = await _db.Collections.FindAsync(collectionId);
            if (collection == null) return NotFound();
            _db.Remove(collection);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<string> UploadImage(string id)
        {
            IFormFile? file = Request.Form.Files[0];
            var result = await _dropboxService.UploadImage(id, file);
            return result;
        }

        //====================== I T E M S =================================

        private Dictionary<string, string> GetCustomFields(Collection collection)
        {
            List<string> customFields = new() { "CustomString1", "CustomString2", "CustomString3",
            "CustomInt1", "CustomInt2", "CustomInt3", "CustomText1", "CustomText2", "CustomText3",
            "CustomBool1", "CustomBool2", "CustomBool3", "CustomDate1", "CustomDate2", "CustomDate3"};
            Dictionary<string, string> result = new();
            foreach(string field in customFields)
                if (collection[field] != null && collection[field].ToString() != "")
                    result.Add(field, collection[field].ToString());
            return result;
        }

        public async Task<IActionResult> Items(string collectionId)
        {
            Collection? collection = await _db.Collections
                .Include(c => c.Items)
                .ThenInclude(i => i.Tags)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == collectionId);
            ViewBag.Collection = collection;
            ViewBag.CustomFields = GetCustomFields(collection);
            ViewBag.Tags = await _db.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem()
        {
            var form = HttpContext.Request.Form;
            List<Tag> tags = new();
            foreach (string tag in form["Tags"].ToString().Split(", "))
            {
                Tag? newTag = await _db.Tags.FindAsync(tag);
                if (newTag == null)
                {
                    newTag = new Tag() { Name = tag };
                    await _db.Tags.AddAsync(newTag);
                }
                tags.Add(newTag);
            }
            Item item = new Item()
            {
                Name = form["Name"],
                Tags = tags,
                CreateDate = DateTime.UtcNow,
                CollectionId = form["collectionId"],
                UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                CustomString1 = form["customString1"],
                CustomString2 = form["customString2"],
                CustomString3 = form["customString3"],
                CustomInt1 = Int32.TryParse(form["customInt1"], out var tempInt1) ? tempInt1 : null,
                CustomInt2 = Int32.TryParse(form["customInt2"], out var tempInt2) ? tempInt2 : null,
                CustomInt3 = Int32.TryParse(form["customInt3"], out var tempInt3) ? tempInt3 : null,
                CustomText1 = form["customText1"],
                CustomText2 = form["customText2"],
                CustomText3 = form["customText3"],
                CustomBool1 = (form["customBool1"] == "on") ? true : false,
                CustomBool2 = (form["customBool2"] == "on") ? true : false,
                CustomBool3 = (form["customBool3"] == "on") ? true : false,
                CustomDate1 = DateTime.TryParse(form["customDate1"], out var tempDate1) ? tempDate1 : null,
                CustomDate2 = DateTime.TryParse(form["customDate2"], out var tempDate2) ? tempDate2 : null,
                CustomDate3 = DateTime.TryParse(form["customDate3"], out var tempDate3) ? tempDate3 : null,
            };
            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();
            return RedirectPermanent($"~/User/Items?collectionId={form["collectionId"]}");
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(string id, string name, string tags,
            string? customString1, string? customString2, string? customString3,
            string? customInt1, string? customInt2, string? customInt3,
            string? customText1, string? customText2, string? customText3,
            string? customBool1, string? customBool2, string? customBool3,
            string? customDate1, string? customDate2, string? customDate3)
        {
            Item? item = await _db.Items.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();
            item.Name = name;
            item.Tags.Clear();
            foreach (string tag in tags.Split(", "))
            {
                Tag? newTag = await _db.Tags.FindAsync(tag);
                if (newTag == null)
                {
                    newTag = new Tag() { Name = tag };
                    await _db.Tags.AddAsync(newTag);
                }
                item.Tags.Add(newTag);
            }
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
            return Ok();
        }
        

        [HttpPost]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            Item? item = await _db.Items.FindAsync(itemId);
            if (item == null) return NotFound();
            _db.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
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
            if (await _db.Likes.FindAsync(itemId, userId) != null) return Ok();
            Like newLike = new Like() { ItemId = itemId, UserId = userId };
            await _db.Likes.AddAsync(newLike);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DislikeItem(string? itemId, string? userId)
        {
            Like? likeToDelete = await _db.Likes.FindAsync(itemId, userId);
            if (likeToDelete == null) return NotFound();
            _db.Remove(likeToDelete);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}