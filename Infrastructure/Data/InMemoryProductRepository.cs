using NewProjectFromScratch.Application.Interfaces;
using NewProjectFromScratch.Domain.Entities;

namespace NewProjectFromScratch.Infrastructure.Data
{
    public sealed class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task AddAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetActiveAsync(string? category, decimal? minPrice, decimal? maxPrice)
        {
            var query = _products.AsEnumerable();

            query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(x => x.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }

            return Task.FromResult(query);
        }

        public Task UpdateAsync(Product product)
        {
            // In-memory list stores the same object instance, so no action is required here.
            return Task.CompletedTask;
        }
    }
}
