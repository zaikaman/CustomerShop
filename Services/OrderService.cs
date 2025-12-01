using Microsoft.EntityFrameworkCore;
using CustomerShop.Data;
using CustomerShop.Models;

namespace CustomerShop.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int customerId, Cart cart, int? promoId, string paymentMethod, string? transferContent = null);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
        Task<Promotion?> ValidatePromoCodeAsync(string promoCode, decimal orderAmount);
        Task<decimal> CalculateDiscountAsync(Promotion promotion, decimal orderAmount);
        Task<bool> CancelOrderAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public OrderService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Order> CreateOrderAsync(int customerId, Cart cart, int? promoId, string paymentMethod, string? transferContent = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var totalAmount = cart.TotalAmount;
                decimal discountAmount = 0;

                // Áp dụng khuyến mãi nếu có
                if (promoId.HasValue)
                {
                    var promo = await context.Promotions.FindAsync(promoId.Value);
                    if (promo != null && promo.IsValid())
                    {
                        discountAmount = await CalculateDiscountAsync(promo, totalAmount);
                        
                        // Cập nhật số lần sử dụng
                        promo.UsedCount++;
                        context.Promotions.Update(promo);
                    }
                }

                // Tạo đơn hàng
                var order = new Order
                {
                    CustomerId = customerId,
                    PromoId = promoId,
                    OrderDate = DateTime.Now,
                    Status = "pending",
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    TransferContent = (paymentMethod == "bank_transfer" || paymentMethod == "e-wallet") ? transferContent : null
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                // Thêm chi tiết đơn hàng và cập nhật tồn kho
                foreach (var item in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Subtotal = item.Subtotal
                    };
                    context.OrderItems.Add(orderItem);

                    // Cập nhật tồn kho
                    var inventory = await context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity -= item.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        context.Inventories.Update(inventory);
                    }
                }

                // Tạo thanh toán
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    Amount = totalAmount - discountAmount,
                    PaymentMethod = paymentMethod,
                    // Tất cả thanh toán đều để pending, nhân viên sẽ xác nhận sau
                    PaymentStatus = "pending",
                    PaymentDate = DateTime.Now
                };
                context.Payments.Add(payment);

                // Đơn hàng luôn ở trạng thái pending (chờ xử lý) để nhân viên xác nhận
                // Không tự động chuyển sang paid

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Orders
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Promotion?> ValidatePromoCodeAsync(string promoCode, decimal orderAmount)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var promo = await context.Promotions
                .FirstOrDefaultAsync(p => p.PromoCode == promoCode);

            if (promo == null)
                return null;

            if (!promo.IsValid())
                return null;

            if (orderAmount < promo.MinOrderAmount)
                return null;

            return promo;
        }

        public Task<decimal> CalculateDiscountAsync(Promotion promotion, decimal orderAmount)
        {
            decimal discount = 0;

            if (promotion.DiscountType == "percent")
            {
                discount = orderAmount * (promotion.DiscountValue / 100);
            }
            else if (promotion.DiscountType == "fixed")
            {
                discount = promotion.DiscountValue;
            }

            // Đảm bảo discount không vượt quá tổng đơn hàng
            if (discount > orderAmount)
            {
                discount = orderAmount;
            }

            return Task.FromResult(discount);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var order = await context.Orders.FindAsync(orderId);
                if (order == null || order.Status != "pending")
                {
                    return false;
                }

                order.Status = "cancelled";
                context.Orders.Update(order);

                // Hoàn lại tồn kho
                var orderItems = await context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync();

                foreach (var item in orderItems)
                {
                    var inventory = await context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity += item.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        context.Inventories.Update(inventory);
                    }
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
