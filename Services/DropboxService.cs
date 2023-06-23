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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Dropbox.Api.Sharing;
using Path = System.IO.Path;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace ToyCollection.Services
{
    public interface IDropboxService
    {
        public Task<string> UploadImage(string id, IFormFile? file);
    }

    public class DropboxService : IDropboxService
    {
        private readonly ILogger<DropboxService> _logger;
        private readonly ApplicationDbContext _db;

        public DropboxService(ILogger<DropboxService> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        private async Task<string> GetAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://api.dropbox.com/oauth2/token"))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("dcwzxh115fgvt4i:ytin6gge64i1dgr"));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    var contentList = new List<string>();
                    contentList.Add("refresh_token=5d_z45947pYAAAAAAAAAAfpaEj2HhEIQnhgMhh3ks-fJjhkZt453vOT7xiZGUKCi");
                    contentList.Add("grant_type=refresh_token");
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var res = await httpClient.SendAsync(request);
                    var json = await res.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    var accessToken = obj["access_token"].Value<string>();
                    return accessToken;
                }
            }
        }

        public async Task<string> UploadImage(string id, IFormFile? file)
        {
            var collection = await _db.Collections.FindAsync(id);
            if (collection == null || file == null || file.Length == 0) return "";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                string filePath = "/" + Path.GetFileName(file.FileName);
                //string accessToken = "KvkoMSvp7aAAAAAAAAAANSoWkqIu1X6J3zh3luPW0IY";
                string accessToken = await GetAccessToken();
                                
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
    }
}