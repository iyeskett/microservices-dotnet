using GeekShopping.Email.Message;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<MySqlContext> _context;

        public EmailRepository(DbContextOptions<MySqlContext> context)
        {
            _context = context;
        }

        public async Task LogEmail(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            EmailLog emailLog = new()
            {
                Email = updatePaymentResultMessage.Email,
                SendDate = DateTime.Now,
                Log = $"Order - {updatePaymentResultMessage.OrderId} has been created succesfully!"
            };

            using var _db = new MySqlContext(_context);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }
    }
}