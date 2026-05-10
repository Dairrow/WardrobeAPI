using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Configurations;

public class OutfitConfiguration : IEntityTypeConfiguration<Outfit>
{
	public void Configure(EntityTypeBuilder<Outfit> builder)
	{
		builder.ToTable("outfits");


		builder.HasKey(x => x.Id);


		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(100);


		builder.HasOne(x => x.User)
			.WithMany(x => x.Outfits)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}