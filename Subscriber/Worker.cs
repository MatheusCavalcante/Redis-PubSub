using Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace Subscriber
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

            await subscriber.SubscribeAsync(Channel, (channel, message) =>
            {
                var messageDeserialized = JsonSerializer.Deserialize<Message>(message);

                _logger.LogInformation("Received message:  {Channel} {@Message}", channel, messageDeserialized);
            });
        }
    }
}