using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToyCollection.Areas.Identity.Data;
using ToyCollection.Models;

namespace ToyCollection.Areas.Identity.Data;

//public class ApplicationDbContext : IdentityDbContext<UserModel, RoleModel, string, IdentityUserClaim<string>,
//    UserRoleModel, IdentityUserLogin<string>,
//    IdentityRoleClaim<string>, IdentityUserToken<string>>
//{
//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//        : base(options)
//    {
//    }

//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder);

//        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

//        builder.Entity<UserRoleModel>(userRole =>
//        {
//            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

//            userRole.HasOne(ur => ur.Role)
//                .WithMany(r => r.UserRoles)
//                .HasForeignKey(ur => ur.RoleId)
//                .IsRequired();

//            userRole.HasOne(ur => ur.User)
//                .WithMany(r => r.UserRoles)
//                .HasForeignKey(ur => ur.UserId)
//                .IsRequired();
//        });
//    }
//}

//public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<UserModel>
//{
//    public void Configure(EntityTypeBuilder<UserModel> builder)
//    {
//        builder.Property(x => x.isBlocked);
//    }
//}

public class ApplicationDbContext : IdentityDbContext<UserModel>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Collection> Collections { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemField> ItemFields { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ItemTag> ItemTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        
        modelBuilder.Entity<ItemField>()
            .HasKey(x => new { x.ItemId, x.FieldId });

        modelBuilder.Entity<ItemField>()
            .HasOne(x => x.Item)
            .WithMany(x => x.ItemFields)
            .HasForeignKey(x => x.ItemId);

        modelBuilder.Entity<ItemField>()
            .HasOne(x => x.Field)
            .WithMany()
            .HasForeignKey(x => x.FieldId);

        modelBuilder.Entity<Field>()
            .HasOne(x => x.Collection)
            .WithMany(x => x.Fields)
            .HasForeignKey(x => x.CollectionId);

        modelBuilder.Entity<Comment>()
            .HasOne(x => x.Item)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ItemId);

        modelBuilder.Entity<Like>()
            .HasOne(x => x.Item)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.ItemId);

        modelBuilder.Entity<ItemTag>()
            .HasKey(x => new { x.ItemId, x.TagId });

        modelBuilder.Entity<ItemTag>()
            .HasOne(x => x.Item)
            .WithMany(x => x.ItemTags)
            .HasForeignKey(x => x.ItemId);

        modelBuilder.Entity<ItemTag>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.ItemTags)
            .HasForeignKey(x => x.TagId);
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.Property(x => x.isBlocked);
    }
}