using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using syll.be.domain.Auth;
using syll.be.domain.DanhBa;
using syll.be.domain.Form;
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
        public DbSet<FormData> FormDatas { get; set; }
        public DbSet<FormDauMuc> FormDauMucs { get; set; }
        public DbSet<FormTruongData> FormTruongDatas { get; set; }
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
            modelBuilder.Entity<FormData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<FormDauMuc>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.Entity<FormTruongData>(entity =>
            {
                entity.Property(e => e.Deleted).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getdate()");
            });
            modelBuilder.HasDefaultSchema(DbSchemas.Core);

            base.OnModelCreating(modelBuilder);
        }
    }
}
