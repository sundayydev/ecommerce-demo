using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Cấu hình Slug
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(255);

        // Slug của Danh mục cũng phải là duy nhất để truy xuất URL (VD: /danh-muc/thiet-bi-dien-tu)
        builder.HasIndex(x => x.Slug)
            .IsUnique();
        
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}