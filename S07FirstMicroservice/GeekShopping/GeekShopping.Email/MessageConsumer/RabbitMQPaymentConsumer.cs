using GeekShopping.Email.Message;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        private string _queueName = "";

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

            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout);
            var quereDeclare = await _channel.QueueDeclareAsync();
            _queueName = quereDeclare.QueueName;
            await _channel.QueueBindAsync(_queueName, ExchangeName, "");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ev) =>
           {
               try
               {
                   using var scope = _serviceProvider.CreateScope();
                   var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
                   var content = Encoding.UTF8.GetString(ev.Body.ToArray());
                   UpdatePaymentResultMessage updatePaymentResultMessage = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
                   await ProcessLogs(updatePaymentResultMessage, repository);
                   await _channel.BasicAckAsync(ev.DeliveryTag, false);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e.Message);
                   throw;
               }
           };
            await _channel.BasicConsumeAsync(queue: _queueName,
                                  autoAck: false,
                                  consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessLogs(UpdatePaymentResultMessage updatePaymentResultMessage, IEmailRepository repository)
        {
            try
            {
                await repository.LogEmail(updatePaymentResultMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}