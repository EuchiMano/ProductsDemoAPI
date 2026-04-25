namespace NewProjectFromScratch.Application.DTOs
{
    public sealed record ProductDto(Guid Id, string Name, decimal Price, int Stock, string Category, bool IsActive);

    public sealed record CreateProductRequest(string Name, decimal Price, int Stock, string Category);

    public sealed record AdjustStockRequest(int QuantityChange);
}
