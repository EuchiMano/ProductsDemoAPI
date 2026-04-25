using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NewProjectFromScratch.Application.Interfaces;
using NewProjectFromScratch.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NewProjectFromScratch.Infrastructure.Messaging.Consumers
{
    public class StockUpdatedConsumer : BackgroundService
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<StockUpdatedConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        private const string QueueName = "products.stock.queue";
        private const int LowStockThreshold = 10;

        public StockUpdatedConsumer(IOptions<RabbitMqSettings> settings, ILogger<StockUpdatedConsumer> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var @event = JsonSerializer.Deserialize<StockUpdatedEvent>(json);

                    if (@event is null)
                    {
                        _logger.LogWarning("Mensaje recibido no pudo deserializarse");
                        await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                        return;
                    }

                    await HandleEventAsync(@event);
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando mensaje de RabbitMQ");
                    await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel!.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,   // manual ack — más seguro
                consumer: consumer
            );

            // Mantener el BackgroundService vivo hasta que se cancele
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private Task HandleEventAsync(StockUpdatedEvent @event)
        {
            if (@event.NewStock < LowStockThreshold)
            {
                _logger.LogWarning(
                    "⚠️ Stock bajo — Producto: {ProductName} ({ProductId}) | Stock anterior: {OldStock} → Stock actual: {NewStock}",
                    @event.ProductName, @event.ProductId, @event.OldStock, @event.NewStock);
            }
            else
            {
                _logger.LogInformation(
                    "📦 Stock actualizado — Producto: {ProductName} ({ProductId}) | {OldStock} → {NewStock}",
                    @event.ProductName, @event.ProductId, @event.OldStock, @event.NewStock);
            }

            return Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // basicQos — procesar un mensaje a la vez
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("StockUpdatedConsumer iniciado, escuchando en '{Queue}'", QueueName);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}