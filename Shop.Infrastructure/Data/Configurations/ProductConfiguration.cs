using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Slug)
               .IsRequired()
               .HasMaxLength(255);
               
        builder.HasIndex(x => x.Slug)
               .IsUnique();
        
        builder.Property(x => x.Description)
               .IsRequired();
        
        builder.Property(x => x.Price)
               .IsRequired()
               .HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasOne(x => x.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.Restrict); 

        // 1-N: 1 Sản phẩm có nhiều Biến thể (Variants - ví dụ: Màu đỏ, màu xanh, size S, M)
        builder.HasMany(x => x.Variants)
               .WithOne(v => v.Product) 
               .HasForeignKey(v => v.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        // 1-N: 1 Sản phẩm có nhiều Hình ảnh (Images)
        builder.HasMany(x => x.Images)
               .WithOne(i => i.Product) 
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
            
    }
}