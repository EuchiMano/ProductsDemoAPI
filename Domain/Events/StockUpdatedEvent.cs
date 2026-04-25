namespace NewProjectFromScratch.Domain.Events
{
    public class StockUpdatedEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}