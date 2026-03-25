using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Tên bảng
        builder.ToTable("ProductImages");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        // Cấu hình IsMain (Ảnh đại diện)
        builder.Property(x => x.IsMain)
            .IsRequired()
            .HasDefaultValue(false); 
        
        // N-1: Nhiều Hình ảnh thuộc về 1 Sản phẩm
        builder.HasOne(x => x.Product)
            .WithMany(p => p.Images) 
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}