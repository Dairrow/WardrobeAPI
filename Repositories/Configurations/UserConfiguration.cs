using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wardrobe.Data.Entities;

namespace Wardrobe.Repositories.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);


        builder.Property(x => x.PasswordHash)
            .IsRequired();


        builder.HasIndex(x => x.Email)
            .IsUnique();


        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}