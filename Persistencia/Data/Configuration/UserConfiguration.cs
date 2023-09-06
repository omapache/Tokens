using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
        .HasMaxLength(3);

        builder.Property(e => e.Username)
        .HasColumnName("Username")
        .HasColumnType("varchar")
        .IsRequired()
        .HasMaxLength(50);

        builder.Property(e => e.Password)
        .HasColumnName("Password")
        .HasColumnType("varchar")
        .IsRequired()
        .HasMaxLength(255);

        builder.Property(e => e.Email)
        .HasColumnName("Email")
        .HasColumnType("varchar")
        .IsRequired()
        .HasMaxLength(150);

        builder
        .HasMany(p => p.Rols)
        .WithMany(p => p.Users)
        .UsingEntity<UserRol>(
          j => j
            .HasOne(pt => pt.Rol)
            .WithMany(t => t.UserRols)
            .HasForeignKey(pt => pt.IdRolFk),
          j => j
            .HasOne(pt => pt.User)
            .WithMany(t => t.UserRols)
            .HasForeignKey(pt => pt.IdUserFk),
          j => 
            {
                j.ToTable("UserRol");
              j.HasKey(t => new {t.IdRolFk, t.IdUserFk});
            });
    }
}
