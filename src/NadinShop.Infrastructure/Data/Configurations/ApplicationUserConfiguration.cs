using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NadinShop.Infrastructure.Data.Entities;

namespace NadinShop.Infrastructure.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(254)")
            .IsRequired();

        builder.Property(user => user.NormalizedEmail)
            .HasColumnName("NormalizedEmail")
            .HasColumnType("nvarchar(254)")
            .IsRequired();

        builder.Property(user => user.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(24)")
            .IsRequired();

        builder.Property(user => user.NormalizedPhone)
            .HasColumnName("NormalizedPhone")
            .HasColumnType("varchar(24)")
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("nvarchar(512)")
            .IsRequired();

        builder.Property(user => user.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(user => user.NormalizedEmail)
            .HasDatabaseName("IX_Users_NormalizedEmail")
            .IsUnique();

        builder.HasIndex(user => user.NormalizedPhone)
            .HasDatabaseName("IX_Users_NormalizedPhone")
            .IsUnique();
    }
}
