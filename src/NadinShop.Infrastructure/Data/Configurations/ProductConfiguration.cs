using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NadinShop.Domain.Entities.Products;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Infrastructure.Data.Configurations;

public class ProductConfiguration:IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(product => product.Name)
            .HasColumnName("Name")
            .HasConversion(name => name.Value, value => ProductName.Create(value))
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(product => product.ProduceDate)
            .HasColumnName("ProduceDate")
            .HasConversion(date => date.Value, value => ProduceDate.Create(value))
            .HasColumnType("date")
            .IsRequired();

        builder.Property(product => product.ManufacturePhone)
            .HasColumnName("ManufacturePhone")
            .HasConversion(phone => phone.Value, value => ManufacturePhone.Create(value))
            .HasColumnType("varchar(32)")
            .IsRequired();

        builder.Property(product => product.ManufactureEmail)
            .HasColumnName("ManufactureEmail")
            .HasConversion(email => email.Value, value => ManufactureEmail.Create(value))
            .HasColumnType("varchar(254)")
            .IsRequired();

        builder.Property(product => product.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasColumnType("bit")
            .IsRequired();

        builder.HasIndex(product => new { product.ManufactureEmail, product.ProduceDate })
            .HasDatabaseName("IX_Products_ManufactureEmail_ProduceDate")
            .IsUnique();

        builder.HasIndex(product => new { product.Name })
            .HasDatabaseName("IX_Products_Name")
            .IsUnique();
    }
}
