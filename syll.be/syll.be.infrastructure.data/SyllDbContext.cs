using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using syll.be.domain.Auth;
using syll.be.domain.ChienDich;
using syll.be.domain.DanhBa;
using syll.be.domain.Form;
using syll.be.domain.FormDanhBa;
using syll.be.domain.ToChuc;
using syll.be.shared.Constants.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.infrastructure.data
{
    public class SyllDbContext : IdentityDbContext<AppUser>
    {
        public SyllDbContext(DbContextOptions<SyllDbContext> options) : base(options)
        {
        }
        public DbSet<ToChuc> ToChucs { get; set; }
        public DbSet<ToChucDanhBa> ToChucDanhBa { get; set; }
        public DbSet<DanhBa> DanhBas { get; set; }
        public DbSet<FormLoai> FormLoais { get; set; }
        public DbSet<FormData> FormDatas { get; set; }
        public DbSet<FormTruongData> FormTruongDatas { get; set; }
        public DbSet<FormDanhBa> FormDanhBa { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<DropDown> DropDowns { get; set; }
        public DbSet<ChienDich> ChienDiches { get; set; }
        public DbSet<ChienDichFormLoai> ChienDichFormLoais { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseOpenIddict();
            modelBuilder.Entity<ToChuc>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<ToChucDanhBa>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DanhBa>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<FormLoai>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<FormData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<FormTruongData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<FormDanhBa>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Block>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Layout>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Row>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<Table>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<DropDown>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<ChienDich>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<ChienDichFormLoai>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.HasDefaultSchema(DbSchemas.Core);

            base.OnModelCreating(modelBuilder);
        }
    }
}
