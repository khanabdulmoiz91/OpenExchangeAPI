using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Infrastructure.Data
{
    public class CurrencyExchangeDbContext : DbContext
    {
        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options)
            : base(options)
        {
        }

        public DbSet<ConversionHistory> ConversionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConversionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FromCurrency)
                    .IsRequired()
                    .HasMaxLength(3);
                    
                entity.Property(e => e.ToCurrency)
                    .IsRequired()
                    .HasMaxLength(3);
                    
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                entity.Property(e => e.ConvertedAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                    
                entity.Property(e => e.ExchangeRate)
                    .HasColumnType("decimal(18,6)")
                    .IsRequired();
                    
                entity.Property(e => e.ClientIp)
                    .HasMaxLength(45); // IPv6 max length
                    
                entity.HasIndex(e => e.ConversionDate);
                entity.HasIndex(e => new { e.FromCurrency, e.ToCurrency });
            });
        }
    }
}