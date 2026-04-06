using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();
            
        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired(false); 
        
        builder.HasOne(r => r.User)
            .WithMany() 
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(r => r.Product)
            .WithMany() 
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasIndex(r => r.ProductId);

        builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
    }
}