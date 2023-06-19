using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ToyCollection.Areas.Identity.Data;

public class UserRoleModel : IdentityUserRole<string>
{
    public virtual UserModel User { get; set; }
    public virtual RoleModel Role { get; set; }
}