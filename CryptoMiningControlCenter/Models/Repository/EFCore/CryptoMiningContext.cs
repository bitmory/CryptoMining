﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CryptoMiningControlCenter.Models.Repository.EFCore
{
    public partial class CryptoMiningContext : DbContext
    {
        public CryptoMiningContext()
        {
        }

        public CryptoMiningContext(DbContextOptions<CryptoMiningContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Miner> Miner { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=212.64.64.36,8989;uid=admin_pro;pwd=cnkj123456!;database=CryptoMining;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Miner>(entity =>
            {
                entity.ToTable("miner");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Currentcalculation)
                    .HasColumnName("currentcalculation")
                    .HasMaxLength(50);

                entity.Property(e => e.Dailycalculation)
                    .HasColumnName("dailycalculation")
                    .HasMaxLength(50);

                entity.Property(e => e.Inactive).HasColumnName("inactive");

                entity.Property(e => e.Link)
                    .HasColumnName("link")
                    .HasMaxLength(300);

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(100);

                entity.Property(e => e.Minertype)
                    .HasColumnName("minertype")
                    .HasMaxLength(100);

                entity.Property(e => e.Pooltype)
                    .HasColumnName("pooltype")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Worker>(entity =>
            {
                entity.ToTable("worker");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Currenthashrate)
                    .IsRequired()
                    .HasColumnName("currenthashrate")
                    .HasMaxLength(100);

                entity.Property(e => e.Dailyhashrate)
                    .IsRequired()
                    .HasColumnName("dailyhashrate")
                    .HasMaxLength(100);

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Poolid).HasColumnName("poolid");

                entity.Property(e => e.Rejected)
                    .IsRequired()
                    .HasColumnName("rejected")
                    .HasMaxLength(100);

                entity.Property(e => e.Updateat)
                    .HasColumnName("updateat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Workername)
                    .IsRequired()
                    .HasColumnName("workername")
                    .HasMaxLength(100);
            });
        }
    }
}