using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDB
{
    public class SharedDbContext : DbContext
    {
        public DbSet<TokenDb> Tokens { get; set; }
        public DbSet<ServerDb> Servers { get; set; }
        public static string CsonnectionString { get; set; } = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SharedDB;";

        // Game Server
        public SharedDbContext()
        {
             
        }

        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
        {

        }
        // Game Server
        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            if(option.IsConfigured == false)
            {
                option
                .UseSqlServer(CsonnectionString);
            }

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TokenDb>()
                .HasIndex(t => t.AccountDbId)
                .IsUnique();

            modelBuilder
                .Entity<ServerDb>()
                .HasIndex(s => s.Name)
                .IsUnique();
        }
    }
}
