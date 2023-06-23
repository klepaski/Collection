using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToyCollection.Areas.Identity.Data;
using ToyCollection.Models;

namespace ToyCollection.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<UserModel>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<Collection> Collections { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        builder.Entity<Theme>()
            .HasKey(x => x.Name);

        builder.Entity<Theme>().HasData(
            new Theme[]
            {
                new Theme { Name="Books" },
                new Theme { Name="Toys" },
                new Theme { Name="Clothes" },
                new Theme { Name="Jewelry" },
                new Theme { Name="Antiques" },
                new Theme { Name="Gnomes" },
                new Theme { Name="Others" }
            });

        builder.Entity<Item>()
            .HasOne(x => x.Collection)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Item>()
            .HasOne(x => x.User)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Collection>()
            .HasOne(x => x.User)
            .WithMany(x => x.Collections)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(x => x.Item)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Like>()
            .HasKey(x => new { x.ItemId, x.UserId });

        builder.Entity<Like>()
            .HasOne(x => x.Item)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Like>()
            .HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Tag>()
            .HasKey(x => x.Name);

        builder.Entity<Tag>()
            .HasMany(x => x.Items)
            .WithMany(x => x.Tags);

        builder.Entity<Tag>().HasData(
            new Tag[]
            {
                new Tag { Name="Barbie" },
                new Tag { Name="Pretty" },
                new Tag { Name="Lermontov" },
                new Tag { Name="Pushkin" },
                new Tag { Name="Garden" },
                new Tag { Name="Stamps" }
            });
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.Property(x => x.isBlocked);
    }
}