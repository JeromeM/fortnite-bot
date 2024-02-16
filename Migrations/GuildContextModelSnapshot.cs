﻿// <auto-generated />
using FortniteBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FortniteBot.Migrations
{
    [DbContext(typeof(GuildContext))]
    partial class GuildContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("FortniteBot.Models.Guild", b =>
                {
                    b.Property<string>("GuildID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GuildID");

                    b.ToTable("Guilds");
                });
#pragma warning restore 612, 618
        }
    }
}
