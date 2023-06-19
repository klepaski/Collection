using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ToyCollection.Areas.Identity.Data;

public class RoleModel : IdentityRole
{
    public ICollection<UserRoleModel> UserRoles { get; set; }
}