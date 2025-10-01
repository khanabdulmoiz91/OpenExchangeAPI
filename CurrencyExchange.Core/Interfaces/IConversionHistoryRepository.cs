using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Core.Interfaces
{
    public interface IConversionHistoryRepository
    {
        Task<ConversionHistory> AddAsync(ConversionHistory history);
        Task<ConversionHistory?> GetByIdAsync(int id);
        Task<IEnumerable<ConversionHistory>> GetAllAsync();
        Task<IEnumerable<ConversionHistory>> GetRecentAsync(int count);
        Task<int> SaveChangesAsync();
    }
}