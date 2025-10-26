using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using syll.be.domain.Auth;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseOpenIddict();
            modelBuilder.HasDefaultSchema(DbSchemas.Core);

            base.OnModelCreating(modelBuilder);
        }
    }
}
