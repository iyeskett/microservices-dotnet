using GeekShopping.PaymentAPI.Message;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;
        private readonly IProcessPayment _processPayment;

        public RabbitMQPaymentConsumer(IServiceProvider serviceProvider, IConfiguration configuration, IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _serviceProvider = serviceProvider;
            _processPayment = processPayment;
            _rabbitMQMessageSender = rabbitMQMessageSender;
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

            await _channel.QueueDeclareAsync(queue: "orderpaymentprocessqueue",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ev) =>
           {
               try
               {
                   var content = Encoding.UTF8.GetString(ev.Body.ToArray());
                   PaymentMessage paymentMessage = JsonSerializer.Deserialize<PaymentMessage>(content);
                   await ProcessPayment(paymentMessage);
                   await _channel.BasicAckAsync(ev.DeliveryTag, false);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e.Message);
                   throw;
               }
           };
            await _channel.BasicConsumeAsync(queue: "orderpaymentprocessqueue",
                                  autoAck: false,
                                  consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessPayment(PaymentMessage paymentMessage)
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentMessage.OrderId,
                Email = paymentMessage.Email
            };

            try
            {
                _rabbitMQMessageSender.SendMessageAsync(updatePaymentResultMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}