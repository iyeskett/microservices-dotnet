using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Message;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private const string ExchangeName = "DirectPaymentUpdateExchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public async void SendMessageAsync(BaseMessage baseMessage)
        {
            if (await ConnectionExistsAsync())
            {
                using var channel = await _connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: false);
                await channel.QueueDeclareAsync(queue: PaymentEmailUpdateQueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                await channel.QueueDeclareAsync(queue: PaymentOrderUpdateQueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                await channel.QueueBindAsync(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");
                await channel.QueueBindAsync(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

                byte[] body = GetMessageAsByteArray(baseMessage);
                await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: "PaymentEmail",
                    body: body);
                await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: "PaymentOrder",
                    body: body);
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage baseMessage)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize((UpdatePaymentResultMessage)baseMessage, options);
            var body = Encoding.UTF8.GetBytes(json);

            return body;
        }

        private async Task<bool> ConnectionExistsAsync()
        {
            if (_connection != null) return true;
            await CreateConnectionAsync();
            return _connection != null;
        }

        private async Task CreateConnectionAsync()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}