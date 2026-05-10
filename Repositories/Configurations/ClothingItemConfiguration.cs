using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Configurations;

public class ClothingItemConfiguration : IEntityTypeConfiguration<ClothingItem>
{
	public void Configure(EntityTypeBuilder<ClothingItem> builder)
	{
		builder.ToTable("clothing_items");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(150);


		builder.Property(x => x.Price)
			.HasPrecision(10, 2);


		builder.HasOne(x => x.Category)
			.WithMany(x => x.ClothingItems)
			.HasForeignKey(x => x.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);


		builder.HasOne(x => x.Brand)
			.WithMany(x => x.ClothingItems)
			.HasForeignKey(x => x.BrandId)
			.OnDelete(DeleteBehavior.Restrict);


		builder.HasOne(x => x.User)
			.WithMany(x => x.ClothingItems)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}