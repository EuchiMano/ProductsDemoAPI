using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NewProjectFromScratch.Application.Interfaces;
using RabbitMQ.Client;

namespace NewProjectFromScratch.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IEventPublisher, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqPublisher> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private async Task EnsureConnectedAsync()
        {
            if (_connection is { IsOpen: true }) return;

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declara el exchange de tipo topic (flexible para múltiples routing keys)
            await _channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declarar la queue
            await _channel.QueueDeclareAsync(
                queue: "products.stock.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Vincularla al exchange
            await _channel.QueueBindAsync(
                queue: "products.stock.queue",
                exchange: _settings.ExchangeName,
                routingKey: "product.stock.#"
            );

            _logger.LogInformation("Conectado a RabbitMQ en {Host}:{Port}", _settings.Host, _settings.Port);
        }

        public async Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default)
        {
            await EnsureConnectedAsync();

            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Evento publicado: {RoutingKey} → {Payload}", routingKey, json);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}