using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Tên bảng
        builder.ToTable("Users");

        // Khóa chính (Kế thừa từ BaseAuditableEntity, thường là Id)
        builder.HasKey(x => x.Id);

        // Cấu hình Email
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);
               
        // Đảm bảo Email không được phép trùng lặp trong hệ thống
        builder.HasIndex(x => x.Email)
            .IsUnique();

        // Cấu hình PasswordHash
        // Chuỗi băm mật khẩu
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        // Cấu hình FullName
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        // Cấu hình Role
        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);
    }
}