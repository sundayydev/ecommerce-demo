using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.IsMain)
            .IsRequired()
            .HasDefaultValue(false); 
        
        builder.HasOne(x => x.Product)
            .WithMany(p => p.Images) 
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}