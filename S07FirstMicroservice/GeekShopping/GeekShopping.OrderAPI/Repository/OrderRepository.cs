using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<MySqlContext> _context;

        public OrderRepository(DbContextOptions<MySqlContext> context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            if (orderHeader == null) return false;
            await using var _db = new MySqlContext(_context);
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderPaymentStatus(long orderHeaderId, bool paid)
        {
            await using var _db = new MySqlContext(_context);
            var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(_ => _.Id == orderHeaderId);
            if (orderHeader == null) return false;
            orderHeader.PaymentStatus = paid;
            _db.OrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}