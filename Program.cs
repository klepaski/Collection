using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToyCollection.Areas.Identity.Data;
using ToyCollection.Services;
using ToyCollection.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<UserModel>(options => options.SignIn.RequireConfirmedAccount = false)
//builder.Services.AddIdentity<UserModel, RoleModel>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequiredLength = 1;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDropboxService, DropboxService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope()) 
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
    string email = "admin@mail.com";
    string password = "1";
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new UserModel();
        user.UserName = "admin";
        user.Email = email;
        user.isBlocked = false;
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
        await userManager.AddToRoleAsync(user, "User");
    }
}

app.MapHub<CommentHub>("/Comment");

app.Run();
