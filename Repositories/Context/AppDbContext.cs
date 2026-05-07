using Microsoft.EntityFrameworkCore;
using System.Data;
using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<ClothingItem> ClothingItems => Set<ClothingItem>();

    public DbSet<Outfit> Outfits => Set<Outfit>();

    public DbSet<OutfitItem> OutfitItems => Set<OutfitItem>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}