using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CryptoMiningWebApi.Models.Repository.EFCore
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

        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<Miner> Miner { get; set; }
        public virtual DbSet<MinerLog> MinerLog { get; set; }
        public virtual DbSet<Pooltype> Pooltype { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=212.64.64.36,8989;uid=mineradmin;pwd=miner123;database=CryptoMining;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("login");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Miner>(entity =>
            {
                entity.ToTable("miner");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Currentcalculation).HasColumnName("currentcalculation");

                entity.Property(e => e.Dailycalculation).HasColumnName("dailycalculation");

                entity.Property(e => e.Dead).HasColumnName("dead");

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

                entity.Property(e => e.Standardcalculation).HasColumnName("standardcalculation");

                entity.Property(e => e.Total).HasColumnName("total");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasMaxLength(50);

                entity.Property(e => e.Updatedate)
                    .HasColumnName("updatedate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<MinerLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK_MinerLog");

                entity.ToTable("miner_log");

                entity.Property(e => e.Link).HasMaxLength(300);

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.MinerType).HasMaxLength(100);

                entity.Property(e => e.PoolType).HasMaxLength(50);

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Unit).HasMaxLength(50);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<Pooltype>(entity =>
            {
                entity.ToTable("pooltype");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(50);
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
