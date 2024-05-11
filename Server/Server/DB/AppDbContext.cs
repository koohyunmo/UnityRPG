using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
    public class AppDbContext : DbContext
    {
        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<ItemDb> Items { get; set; }
        public DbSet<MailDb> Mails { get; set;}

        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDB;";

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            string dbServerPath = ConfigManager.Config != null ? ConfigManager.Config.connectionString : _connectionString;
            Console.WriteLine($"DB Server : {dbServerPath}");
            option
                //.UseLoggerFactory(_logger)
                .UseSqlServer(dbServerPath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            modelBuilder
               .Entity<PlayerDb>()
               .HasIndex(p => p.PlayerName)
               .IsUnique();

            modelBuilder
                .Entity<ItemDb>()
                .HasIndex(i => i.TemplateId);
        }
    }
}
