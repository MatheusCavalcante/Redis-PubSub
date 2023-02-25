using Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace Publisher
{
    public class Worker : BackgroundService
    {
        private static readonly string ConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer Connection = ConnectionMultiplexer.Connect(ConnectionString);
        private const string Channel = "redis-channel";

        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = Connection.GetSubscriber();

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new Message(Guid.NewGuid(), DateTime.UtcNow);

                var json = JsonSerializer.Serialize(message);

                await subscriber.PublishAsync(Channel, json);

                _logger.LogInformation("Sending message: {@Message}", message);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}