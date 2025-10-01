using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Infrastructure.Data;

namespace CurrencyExchange.Infrastructure.Repositories
{
    public class ConversionHistoryRepository : IConversionHistoryRepository
    {
        private readonly CurrencyExchangeDbContext _context;

        public ConversionHistoryRepository(CurrencyExchangeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ConversionHistory> AddAsync(ConversionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            await _context.ConversionHistories.AddAsync(history);
            return history;
        }

        public async Task<ConversionHistory?> GetByIdAsync(int id)
        {
            return await _context.ConversionHistories
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<ConversionHistory>> GetAllAsync()
        {
            return await _context.ConversionHistories
                .OrderByDescending(h => h.ConversionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConversionHistory>> GetRecentAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero", nameof(count));

            return await _context.ConversionHistories
                .OrderByDescending(h => h.ConversionDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}