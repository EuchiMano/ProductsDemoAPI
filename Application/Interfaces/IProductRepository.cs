using NewProjectFromScratch.Domain.Entities;

namespace NewProjectFromScratch.Application.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetActiveAsync(string? category, decimal? minPrice, decimal? maxPrice);
        Task UpdateAsync(Product product);
    }
}
