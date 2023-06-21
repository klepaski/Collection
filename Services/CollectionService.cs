using Azure.Core;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToyCollection.Areas.Identity.Data;
using ToyCollection.Models;
using static Dropbox.Api.Common.PathRoot;
using static Dropbox.Api.Files.ListRevisionsMode;
using static Dropbox.Api.Files.SearchMatchType;
using Path = System.IO.Path;

namespace ToyCollection.Services
{
    public interface ICollectionService
    {
        public List<Theme> GetThemes();
        public List<Collection> GetCollectionsOfUser(ClaimsPrincipal userPrincipal);
        public void CreateCollection(Params parms, ClaimsPrincipal userPrincipal);
        public void DeleteCollection(string id);
        public void EditCollection(string id, Params parms);
        public Task<string> UploadImage(Guid id, IFormFile? file);

        public List<Item> GetItems(string id);
        public Dictionary<string, string> GetCustomFields(string id);
        public void CreateItem(string name, string collectionId, Dictionary<string, string> parms, ClaimsPrincipal userPrincipal);
        public void DeleteItem(string id);
        public string GetCollectionOfItem(string id);

        public void EditItem(string id, string name, string tags,
             string? customString1, string? customString2, string? customString3,
             string? customInt1, string? customInt2, string? customInt3,
             string? customText1, string? customText2, string? customText3,
             string? customBool1, string? customBool2, string? customBool3,
             string? customDate1, string? customDate2, string? customDate3);
    }

    public class CollectionService : ICollectionService
    {
        private readonly ILogger<CollectionService> _logger;
        private readonly ApplicationDbContext _db;

        private string[] keys = new string[] {"CustomString1", "CustomString2", "CustomString3",
                "CustomInt1", "CustomInt2", "CustomInt3", "CustomText1", "CustomText2", "CustomText3",
                "CustomBool1", "CustomBool2", "CustomBool3", "CustomDate1", "CustomDate2", "CustomDate3" };

        public CollectionService(ILogger<CollectionService> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public List<Theme> GetThemes() => _db.Themes.ToList();

        public List<Collection> GetCollectionsOfUser(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.Find(userId);
            return _db.Collections.Where(x => x.User == user).ToList();
        }


        public void CreateCollection(Params parms, ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _db.Users.Find(userId);
            Collection? collection = new Collection()
            {
                User = user,
                Name = parms.name,
                Description = parms.description,
                Theme = parms.theme,
                CustomString1 = parms.customString1,
                CustomString2 = parms.customString2,
                CustomString3 = parms.customString3,
                CustomInt1 = parms.customInt1,
                CustomInt2 = parms.customInt2,
                CustomInt3 = parms.customInt3,
                CustomText1 = parms.customText1,
                CustomText2 = parms.customText2,
                CustomText3 = parms.customText3,
                CustomBool1 = parms.customBool1,
                CustomBool2 = parms.customBool2,
                CustomBool3 = parms.customBool3,
                CustomDate1 = parms.customDate1,
                CustomDate2 = parms.customDate2,
                CustomDate3 = parms.customDate3
            };
            _db.Collections.Add(collection);
            _db.SaveChanges();
        }

        public void DeleteCollection(string id)
        {
            Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (collection == null) return;
            _db.Remove(collection);
            _db.SaveChanges();
        }


        public void EditCollection(string id, Params parms)
        {
            Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (collection == null) return;
            collection.Name = parms.name;
            collection.Description = parms.description;
            collection.Theme = parms.theme;
            collection.CustomString1 = parms.customString1;
            collection.CustomString2 = parms.customString2;
            collection.CustomString3 = parms.customString3;
            collection.CustomInt1 = parms.customInt1;
            collection.CustomInt2 = parms.customInt2;
            collection.CustomInt3 = parms.customInt3;
            collection.CustomText1 = parms.customText1;
            collection.CustomText2 = parms.customText2;
            collection.CustomText3 = parms.customText3;
            collection.CustomBool1 = parms.customBool1;
            collection.CustomBool2 = parms.customBool2;
            collection.CustomBool3 = parms.customBool3;
            collection.CustomDate1 = parms.customDate1;
            collection.CustomDate2 = parms.customDate2;
            collection.CustomDate3 = parms.customDate3;
            _db.SaveChanges();
        }

        public async Task<string> UploadImage(Guid id, IFormFile? file)
        {
            var collection = await _db.Collections.FindAsync(id);
            if (collection == null || file == null || file.Length == 0) return "";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                string filePath = "/" + Path.GetFileName(file.FileName);
                string accessToken = "sl.BgsNBzjLfyaOk49e-vpKkp8JZybwFgiQwfizSkEYrRRK-FlLe1zbFrr7eaY3m5Lfa_XLApoocOptTJg1dggFp0zRfAXWVfmMJ5nIdXF_G3LuZDXFe4lHfByV4tLzHWWXCIG_iH69Y6s";
                using (var dbx = new DropboxClient(accessToken))
                {
                    stream.Position = 0;
                    var uploaded = await dbx.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: stream);
                    var shareLink = await dbx.Sharing.CreateSharedLinkAsync(filePath);
                    var pictureUrl = shareLink.Url.Replace("www.dropbox.com", "dl.dropboxusercontent.com").Replace("dl=0", "raw=1");
                    collection.ImageUrl = pictureUrl;
                    await _db.SaveChangesAsync();
                    return pictureUrl;
                }
            }
        }

        public List<Item> GetItems(string id)
        {
            var items = _db.Items.Where(x => x.CollectionId.ToString() == id);
            return items.ToList();
        }

        public Dictionary<string, string> GetCustomFields(string id)
        {
            Dictionary<string, string> result = new();
            Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (collection == null) return result;
            //string[] keys = new string[] {"CustomString1", "CustomString2", "CustomString3",
            //    "CustomInt1", "CustomInt2", "CustomInt3",
            //    "CustomText1", "CustomText2", "CustomText3",
            //    "CustomBool1", "CustomBool2", "CustomBool3",
            //    "CustomDate1", "CustomDate2", "CustomDate3" };
            foreach (string key in keys)
            {
                if (collection[key] != "" && collection[key] != null)
                {
                    result.Add(key, collection[key].ToString());
                }
            }
            return result;
        }

        public void EditItem(string id, string name, string tags,
            string? customString1, string? customString2, string? customString3,
            string? customInt1, string? customInt2, string? customInt3,
            string? customText1, string? customText2, string? customText3,
            string? customBool1, string? customBool2, string? customBool3,
            string? customDate1, string? customDate2, string? customDate3)
        {
            Item? item = _db.Items.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (item == null) return;
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
            _db.SaveChanges();
        }

        public void CreateItem(string name, string collectionId, Dictionary<string, string> parms, ClaimsPrincipal userPrincipal)
        {
            Item item = new Item();
            item.Name = name;
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            item.User = _db.Users.Find(userId);
            item.Collection = _db.Collections.FirstOrDefault(x => x.Id.ToString().Equals(collectionId));

            foreach (var parm in parms)
            {
                item[parm.Key] = parm.Key switch
                {
                    string a when a.Contains("Int") => Int32.Parse(parm.Value),
                    string b when b.Contains("Bool") => (parm.Value == "on") ? true : false,
                    string c when c.Contains("Date") => DateTime.Parse(parm.Value),
                    _ => parm.Value
                };

                //if (parm.Key.Contains("Int"))
                //{
                //    item[parm.Key] = Int32.Parse(parm.Value);
                //}
                //else if (parm.Key.Contains("Bool"))
                //{
                //    item[parm.Key] = (parm.Value == "on") ? true : false;
                //}
                //else if (parm.Key.Contains("Date"))
                //{
                //    item[parm.Key] = DateTime.Parse(parm.Value);
                //}
                //else
                //{
                //    item[parm.Key] = parm.Value;
                //}
            }
            _db.Items.Add(item);
            _db.SaveChanges();
        }

        public void DeleteItem(string id)
        {
            Item? item = _db.Items.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if (item == null) return;
            _db.Remove(item);
            _db.SaveChanges();
        }

        public string GetCollectionOfItem(string id)
        {
            Item? item = _db.Items.FirstOrDefault(x => x.Id.ToString().Equals(id));
            Collection? collection = _db.Collections.FirstOrDefault(x => x.Id.Equals(item.CollectionId));
            if (collection == null) return "";
            return collection.Id.ToString();
        }
    }
}