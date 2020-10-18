using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DeveloperCase.Models
{
    public partial class SalesContext : DbContext
    {
        public SalesContext()
        {
        }

        public SalesContext(DbContextOptions<SalesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<TblErrorLog> TblErrorLog { get; set; }
        public virtual DbSet<ViewSalesProfits> ViewSalesProfits { get; set; }
        public virtual DbSet<ViewTotalProfit> ViewTotalProfit { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=.;Database=DeveloperCase;User Id=sa;password=root85;Trusted_Connection=False;MultipleActiveResultSets=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.Country)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ItemType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderPriority)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.Region)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SalesChannel)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.ShipDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblErrorLog>(entity =>
            {
                entity.ToTable("tblErrorLog");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Module)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ViewSalesProfits>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ViewSalesProfits");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ViewTotalProfit>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ViewTotalProfit");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
