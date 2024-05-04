using Microsoft.EntityFrameworkCore;

namespace MarketServer.DB
{
    public class MarketAppDbContext : DbContext
    {

        public DbSet<MarketDb> MarketItems { get; set; }

        public MarketAppDbContext(DbContextOptions<MarketAppDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<MarketDb>()
                .HasIndex(m => m.MarketDbId)
                .IsUnique();

            modelBuilder
                .Entity<MarketDb>()
                .HasIndex(m => m.ItemName);
        }
    }
}
