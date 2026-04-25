namespace NewProjectFromScratch.Domain.Entities
{
    public sealed class Product
    {
        private Product(Guid id, string name, decimal price, int stock, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            Category = category;
            IsActive = true;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Category { get; private set; }
        public bool IsActive { get; private set; }

        public static Product Create(string name, decimal price, int stock, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            if (stock < 0)
            {
                throw new ArgumentException("Stock must be greater than or equal to zero.", nameof(stock));
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category is required.", nameof(category));
            }

            return new Product(Guid.NewGuid(), name.Trim(), price, stock, category.Trim());
        }

        public void AdjustStock(int quantityChange)
        {
            var newStock = Stock + quantityChange;

            if (newStock < 0)
            {
                throw new InvalidOperationException("Stock adjustment would result in negative stock.");
            }

            Stock = newStock;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
