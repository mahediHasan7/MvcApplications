﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MvcApp1.DataAccess.Data;

#nullable disable

namespace MvcApp1.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MvcApp1.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Web Development"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Mobile Development"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Game Development"
                        },
                        new
                        {
                            Id = 4,
                            DisplayOrder = 4,
                            Name = "Machine Learning"
                        },
                        new
                        {
                            Id = 5,
                            DisplayOrder = 5,
                            Name = "Artificial Intelligence"
                        },
                        new
                        {
                            Id = 6,
                            DisplayOrder = 6,
                            Name = "Data Science"
                        },
                        new
                        {
                            Id = 7,
                            DisplayOrder = 7,
                            Name = "Cloud Computing"
                        },
                        new
                        {
                            Id = 8,
                            DisplayOrder = 8,
                            Name = "Cyber Security"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}