using Microsoft.Extensions.Logging.Abstractions;
using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Application.Services;
using NewProjectFromScratch.Infrastructure.Data;
using Xunit;

namespace NewProjectFromScratch.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateProductAsync_ShouldReturnCreatedProduct()
        {
            var repository = new InMemoryProductRepository();
            var service = new ProductService(repository, NullLogger<ProductService>.Instance);

            var request = new CreateProductRequest("Test product", 12.50m, 10, "Tools");
            var product = await service.CreateProductAsync(request);

            Assert.Equal("Test product", product.Name);
            Assert.Equal(12.50m, product.Price);
            Assert.Equal(10, product.Stock);
            Assert.Equal("Tools", product.Category);
            Assert.True(product.IsActive);
        }

        [Fact]
        public async Task AdjustStockAsync_ShouldUpdateProductStock()
        {
            var repository = new InMemoryProductRepository();
            var service = new ProductService(repository, NullLogger<ProductService>.Instance);

            var createRequest = new CreateProductRequest("Widget", 5.00m, 10, "Gadgets");
            var product = await service.CreateProductAsync(createRequest);

            var updated = await service.AdjustStockAsync(product.Id, -4);

            Assert.Equal(6, updated.Stock);
            Assert.Equal(product.Id, updated.Id);
        }

        [Fact]
        public async Task DeactivateProductAsync_ShouldMarkProductAsInactive()
        {
            var repository = new InMemoryProductRepository();
            var service = new ProductService(repository, NullLogger<ProductService>.Instance);

            var createRequest = new CreateProductRequest("DeactivateMe", 2.00m, 5, "Misc");
            var product = await service.CreateProductAsync(createRequest);

            await service.DeactivateProductAsync(product.Id);
            var stored = await repository.GetByIdAsync(product.Id);

            Assert.NotNull(stored);
            Assert.False(stored!.IsActive);
        }
    }
}
