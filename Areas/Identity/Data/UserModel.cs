using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ToyCollection.Models;

namespace ToyCollection.Areas.Identity.Data;

public class UserModel : IdentityUser
{
    public bool isBlocked { get; set; }

    public List<Collection> Collections { get; set; } = new();
    public List<Item> Items { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
}