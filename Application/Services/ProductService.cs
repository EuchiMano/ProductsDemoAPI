using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Application.Interfaces;
using NewProjectFromScratch.Domain.Entities;
using NewProjectFromScratch.Domain.Events;

namespace NewProjectFromScratch.Application.Services
{
    public sealed class ProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;
         private readonly IEventPublisher _eventPublisher;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<Product> CreateProductAsync(CreateProductRequest request)
        {
            var product = Product.Create(request.Name, request.Price, request.Stock, request.Category);
            await _repository.AddAsync(product);

            _logger.LogInformation("Created product {ProductId} with name {ProductName} and stock {Stock}", product.Id, product.Name, product.Stock);
            return product;
        }

        public Task<IEnumerable<Product>> GetActiveProductsAsync(string? category, decimal? minPrice, decimal? maxPrice)
        {
            return _repository.GetActiveAsync(category, minPrice, maxPrice);
        }

        public async Task<Product> AdjustStockAsync(Guid id, int quantityChange)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null || !product.IsActive)
            {
                _logger.LogWarning("Attempted to adjust stock for missing or inactive product {ProductId}", id);
                throw new KeyNotFoundException("Product not found.");
            }

            var oldStock = product.Stock;
            product.AdjustStock(quantityChange);
            await _repository.UpdateAsync(product);

            _logger.LogInformation("Adjusted stock for product {ProductId} from {OldStock} to {NewStock}", product.Id, oldStock, product.Stock);
            var @event = new StockUpdatedEvent
            {
            ProductId = product.Id,
            ProductName = product.Name,
            OldStock = oldStock,
            NewStock = product.Stock,
            OccurredAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(@event, routingKey: "product.stock.updated");
            return product;
        }

        public async Task DeactivateProductAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null || !product.IsActive)
            {
                _logger.LogWarning("Attempted to deactivate missing or inactive product {ProductId}", id);
                throw new KeyNotFoundException("Product not found.");
            }

            product.Deactivate();
            await _repository.UpdateAsync(product);
            _logger.LogInformation("Deactivated product {ProductId}", product.Id);
        }
    }
}
