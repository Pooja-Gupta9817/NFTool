

using DesktopTool.App.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();// Table name
        public DbSet<UploadedFiles> UploadedFiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        // Optional: for manual configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // add Fluent API here if needed
        }
    }

}
