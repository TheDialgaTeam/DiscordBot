﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheDialgaTeam.Discord.Bot.Services.EntityFramework;

namespace TheDialgaTeam.Discord.Bot.Migrations
{
    [DbContext(typeof(SqliteContext))]
    partial class SqliteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordAppOwnerTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong?>("DiscordAppId");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("DiscordAppId");

                    b.ToTable("DiscordAppOwnerTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordAppTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppDescription");

                    b.Property<string>("AppName");

                    b.Property<string>("BotToken")
                        .IsRequired();

                    b.Property<ulong>("ClientId");

                    b.Property<string>("ClientSecret")
                        .IsRequired();

                    b.Property<DateTimeOffset?>("LastUpdateCheck");

                    b.HasKey("Id");

                    b.ToTable("DiscordAppTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordChannelModeratorTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordChannelId");

                    b.Property<int>("Type");

                    b.Property<ulong>("Value");

                    b.HasKey("Id");

                    b.HasIndex("DiscordChannelId");

                    b.ToTable("DiscordChannelModeratorTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordChannelTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<ulong>("DiscordGuildId");

                    b.HasKey("Id");

                    b.HasIndex("DiscordGuildId");

                    b.ToTable("DiscordChannelTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildModeratorTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordGuildId");

                    b.Property<int>("Type");

                    b.Property<ulong>("Value");

                    b.HasKey("Id");

                    b.HasIndex("DiscordGuildId");

                    b.ToTable("DiscordGuildModeratorTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildModuleTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<ulong>("DiscordGuildId");

                    b.Property<string>("Module")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DiscordGuildId");

                    b.ToTable("DiscordGuildModuleTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildTable", b =>
                {
                    b.Property<ulong?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordAppId");

                    b.Property<ulong>("GuildId");

                    b.Property<string>("Prefix");

                    b.HasKey("Id");

                    b.HasIndex("DiscordAppId");

                    b.ToTable("DiscordGuildTable");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordAppOwnerTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordAppTable", "DiscordApp")
                        .WithMany("DiscordAppOwners")
                        .HasForeignKey("DiscordAppId");
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordChannelModeratorTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordChannelTable", "DiscordChannel")
                        .WithMany("DiscordChannelModerators")
                        .HasForeignKey("DiscordChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordChannelTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildTable", "DiscordGuild")
                        .WithMany("DiscordChannels")
                        .HasForeignKey("DiscordGuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildModeratorTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildTable", "DiscordGuild")
                        .WithMany("DiscordGuildModerators")
                        .HasForeignKey("DiscordGuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildModuleTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildTable", "DiscordGuild")
                        .WithMany("DiscordGuildModules")
                        .HasForeignKey("DiscordGuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordGuildTable", b =>
                {
                    b.HasOne("TheDialgaTeam.Discord.Bot.Models.EntityFramework.DiscordAppTable", "DiscordApp")
                        .WithMany("DiscordGuilds")
                        .HasForeignKey("DiscordAppId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}