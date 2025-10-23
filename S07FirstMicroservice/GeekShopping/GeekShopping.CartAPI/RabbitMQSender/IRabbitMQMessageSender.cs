using GeekShopping.MessageBus;

namespace GeekShopping.CartAPI.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        void SendMessageAsync(BaseMessage baseMessage, string queueName);
    }
}