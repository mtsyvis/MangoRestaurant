using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddOrderAsync(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(dbContext);
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatusAsync(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(dbContext);
            var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }
        }
    }
}
