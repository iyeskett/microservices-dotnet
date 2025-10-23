using GeekShopping.OrderAPI.Model;

namespace GeekShopping.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);

        Task<bool> UpdateOrderPaymentStatus(long orderHeaderId, bool paid);
    }
}