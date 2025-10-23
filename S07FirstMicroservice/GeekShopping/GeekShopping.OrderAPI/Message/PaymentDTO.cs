using GeekShopping.MessageBus;

namespace GeekShopping.OrderAPI.Message
{
    public class PaymentDTO : BaseMessage
    {
        public long OrderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string ExpireMonthYear { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal PurchaseAmount { get; set; }
    }
}