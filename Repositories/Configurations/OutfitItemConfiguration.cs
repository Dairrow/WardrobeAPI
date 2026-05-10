using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Configurations;

public class OutfitItemConfiguration : IEntityTypeConfiguration<OutfitItem>
{
	public void Configure(EntityTypeBuilder<OutfitItem> builder)
	{
		builder.ToTable("outfit_items");


		builder.HasKey(x => new
		{
			x.OutfitId,
			x.ClothingItemId
		});


		builder.HasOne(x => x.Outfit)
			.WithMany(x => x.OutfitItems)
			.HasForeignKey(x => x.OutfitId)
			.OnDelete(DeleteBehavior.Cascade);


		builder.HasOne(x => x.ClothingItem)
			.WithMany(x => x.OutfitItems)
			.HasForeignKey(x => x.ClothingItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}