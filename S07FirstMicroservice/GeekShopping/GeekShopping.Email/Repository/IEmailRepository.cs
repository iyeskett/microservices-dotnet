using GeekShopping.Email.Message;

namespace GeekShopping.Email.Repository
{
    public interface IEmailRepository
    {
        Task LogEmail(UpdatePaymentResultMessage updatePaymentResultMessage);
    }
}