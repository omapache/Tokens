﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistencia;

#nullable disable

namespace Persistencia.Data.Migrations
{
    [DbContext(typeof(DbApiContext))]
    [Migration("20230906200723_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Dominio.Entities.Rol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(3)
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Nombre");

                    b.HasKey("Id");

                    b.ToTable("rol", (string)null);
                });

            modelBuilder.Entity("Dominio.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(3)
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar")
                        .HasColumnName("Email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar")
                        .HasColumnName("Password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar")
                        .HasColumnName("Username");

                    b.HasKey("Id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Dominio.Entities.UserRol", b =>
                {
                    b.Property<int>("IdRolFk")
                        .HasColumnType("int");

                    b.Property<int>("IdUserFk")
                        .HasColumnType("int");

                    b.HasKey("IdRolFk", "IdUserFk");

                    b.HasIndex("IdUserFk");

                    b.ToTable("UserRol", (string)null);
                });

            modelBuilder.Entity("Dominio.Entities.UserRol", b =>
                {
                    b.HasOne("Dominio.Entities.Rol", "Rol")
                        .WithMany("UserRols")
                        .HasForeignKey("IdRolFk")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dominio.Entities.User", "User")
                        .WithMany("UserRols")
                        .HasForeignKey("IdUserFk")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Dominio.Entities.Rol", b =>
                {
                    b.Navigation("UserRols");
                });

            modelBuilder.Entity("Dominio.Entities.User", b =>
                {
                    b.Navigation("UserRols");
                });
#pragma warning restore 612, 618
        }
    }
}
