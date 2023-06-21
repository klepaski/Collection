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
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Dropbox.Api.Files;
using Dropbox.Api;
using static Dropbox.Api.Files.SearchMatchType;
using static Dropbox.Api.Files.ListRevisionsMode;

namespace ToyCollection.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ICollectionService _collectionService;

        public UserController(ILogger<UserController> logger, ICollectionService collectionService)
        {
            _logger = logger;
            _collectionService = collectionService;
        }

        public IActionResult Index()
        {
            ViewBag.Themes = _collectionService.GetThemes();
            List<Collection> collections = _collectionService.GetCollectionsOfUser(HttpContext.User);
            return View(collections);
        }

        [HttpPost]
        public IActionResult CreateCollection()
        {
            var form = HttpContext.Request.Form;
            Params parms = new Params(
                form["Name"],
                form["Description"],
                form["Theme"],
                form["CustomString1"],
                form["CustomString2"],
                form["CustomString3"],
                form["CustomInt1"],
                form["CustomInt2"],
                form["CustomInt3"],
                form["CustomText1"],
                form["CustomText2"],
                form["CustomText3"],
                form["CustomBool1"],
                form["CustomBool2"],
                form["CustomBool3"],
                form["CustomDate1"],
                form["CustomDate2"],
                form["CustomDate3"]
            );
            _collectionService.CreateCollection(parms, HttpContext.User);
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public IActionResult EditCollection(string id, string name, string description, string theme,
            string customString1, string customString2, string customString3,
            string customInt1, string customInt2, string customInt3,
            string customText1, string customText2, string customText3,
            string customBool1, string customBool2, string customBool3,
            string customDate1, string customDate2, string customDate3)
        {
            var parms = new Params(name, description, theme, 
                customString1, customString2, customString3, 
                customInt1, customInt2, customInt3, 
                customText1, customText2, customText3, 
                customBool1, customBool2, customBool3, 
                customDate1, customDate2, customDate3);
            _collectionService.EditCollection(id, parms);
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public IActionResult DeleteCollection()
        {
            var id = HttpContext.Request.Form["collectionId"];
            _collectionService.DeleteCollection(id);
            return RedirectPermanent("~/User/Index");
        }

        [HttpPost]
        public async Task<string> UploadImage(Guid id)
        {
            IFormFile? file = Request.Form.Files[0];
            var result = await _collectionService.UploadImage(id, file);
            return result;
        }

        public IActionResult Items(string id)
        {
            ViewBag.CollectionId = id;
            ViewBag.Items = _collectionService.GetItems(id);
            ViewBag.CustomFields = _collectionService.GetCustomFields(id);
            return View();
        }

        [HttpPost]
        public IActionResult CreateItem()
        {
            Dictionary<string, string> parms = new();
            var form = HttpContext.Request.Form;
            var collectionId = form["collectionId"];
            var customFields = _collectionService.GetCustomFields(collectionId);
            var name = form["Name"];
            foreach (var field in customFields)
            {
                parms.Add(field.Key, form[field.Key]);
            }
            _collectionService.CreateItem(name, collectionId, parms, HttpContext.User);
            return RedirectPermanent($"~/User/Items?id={collectionId}");
        }

        [HttpPost]
        public IActionResult EditItem(string id, string name, string tags,
            string? customString1, string? customString2, string? customString3,
            string? customInt1, string? customInt2, string? customInt3,
            string? customText1, string? customText2, string? customText3,
            string? customBool1, string? customBool2, string? customBool3,
            string? customDate1, string? customDate2, string? customDate3)
        {
            _collectionService.EditItem(id, name, tags, 
                customString1, customString2, customString3,
                customInt1, customInt2, customInt3,
                customText1, customText2, customText3,
                customBool1, customBool2, customBool3,
                customDate1, customDate2, customDate3);

            var collectionId = _collectionService.GetCollectionOfItem(id);
            return RedirectPermanent($"~/User/Items?id={collectionId}");
        }

        [HttpPost]
        public IActionResult DeleteItem()
        {
            var id = HttpContext.Request.Form["itemId"];
            var collectionId = _collectionService.GetCollectionOfItem(id);
            _collectionService.DeleteItem(id);
            return RedirectPermanent($"~/User/Items?id={collectionId}");
        }
    }
}