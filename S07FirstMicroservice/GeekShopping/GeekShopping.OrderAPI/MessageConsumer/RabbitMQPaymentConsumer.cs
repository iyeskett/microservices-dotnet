using GeekShopping.OrderAPI.Message;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;
        private const string ExchangeName = "DirectPaymentUpdateExchange";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        public RabbitMQPaymentConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct);
            await _channel.QueueDeclareAsync(PaymentOrderUpdateQueueName, false, false, false, null);
            await _channel.QueueBindAsync(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ev) =>
           {
               try
               {
                   using var scope = _serviceProvider.CreateScope();
                   var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                   var content = Encoding.UTF8.GetString(ev.Body.ToArray());
                   UpdatePaymentResultDTO updatePaymentResultDTO = JsonSerializer.Deserialize<UpdatePaymentResultDTO>(content);
                   await UpdatePaymentStatus(updatePaymentResultDTO, repository);
                   await _channel.BasicAckAsync(ev.DeliveryTag, false);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e.Message);
                   throw;
               }
           };
            await _channel.BasicConsumeAsync(queue: PaymentOrderUpdateQueueName,
                                  autoAck: false,
                                  consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultDTO updatePaymentResultDTO, IOrderRepository repository)
        {
            try
            {
                await repository.UpdateOrderPaymentStatus(updatePaymentResultDTO.OrderId, updatePaymentResultDTO.Status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}