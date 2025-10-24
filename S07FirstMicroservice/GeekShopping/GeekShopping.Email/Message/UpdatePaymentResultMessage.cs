namespace GeekShopping.Email.Message
{
    public class UpdatePaymentResultMessage
    {
        public long OrderId { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}