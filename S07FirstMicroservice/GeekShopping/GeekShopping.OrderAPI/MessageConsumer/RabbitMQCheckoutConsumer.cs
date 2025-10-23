using GeekShopping.OrderAPI.Message;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQCheckoutConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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

            await _channel.QueueDeclareAsync(queue: "checkoutqueue",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ev) =>
           {
               try
               {
                   using var scope = _serviceProvider.CreateScope();
                   var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                   var content = Encoding.UTF8.GetString(ev.Body.ToArray());
                   CheckoutDTO checkoutDTO = JsonSerializer.Deserialize<CheckoutDTO>(content);
                   await ProcessOrder(checkoutDTO, repository);
                   await _channel.BasicAckAsync(ev.DeliveryTag, false);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e.Message);
                   throw;
               }
              
           };
            await _channel.BasicConsumeAsync(queue: "checkoutqueue",
                                  autoAck: false,
                                  consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessOrder(CheckoutDTO checkoutDTO, IOrderRepository repository)
        {
            OrderHeader orderHeader = new()
            {
                UserId = checkoutDTO.UserId,
                FirstName = checkoutDTO.FirstName,
                LastName = checkoutDTO.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = checkoutDTO.CardNumber,
                CouponCode = checkoutDTO.CouponCode,
                CVV = checkoutDTO.CVV,
                DiscountAmount = checkoutDTO.DiscountAmount,
                Email = checkoutDTO.Email,
                ExpiryMonthYear = checkoutDTO.ExpiryMothYear,
                OrderTime = DateTime.Now,
                PurchaseAmount = checkoutDTO.PurchaseAmount,
                PaymentStatus = false,
                Phone = checkoutDTO.Phone,
                DateTime = checkoutDTO.DateTime
            };

            foreach (var details in checkoutDTO.CartDetailDTOs)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product.Name,
                    Price = details.Product.Price,
                    Count = details.Count,
                };
                orderHeader.OrderTotalItems += details.Count;
                orderHeader.OrderDetails.Add(orderDetail);
            }

            await repository.AddOrder(orderHeader);
        }
    }
}