using Microsoft.EntityFrameworkCore;
using CustomerShop.Data;
using CustomerShop.Models;

namespace CustomerShop.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int customerId, Cart cart, int? promoId, string paymentMethod);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
        Task<Promotion?> ValidatePromoCodeAsync(string promoCode, decimal orderAmount);
        Task<decimal> CalculateDiscountAsync(Promotion promotion, decimal orderAmount);
        Task<bool> CancelOrderAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(int customerId, Cart cart, int? promoId, string paymentMethod)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var totalAmount = cart.TotalAmount;
                decimal discountAmount = 0;

                // Áp dụng khuyến mãi nếu có
                if (promoId.HasValue)
                {
                    var promo = await _context.Promotions.FindAsync(promoId.Value);
                    if (promo != null && promo.IsValid())
                    {
                        discountAmount = await CalculateDiscountAsync(promo, totalAmount);
                        
                        // Cập nhật số lần sử dụng
                        promo.UsedCount++;
                        _context.Promotions.Update(promo);
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
                    DiscountAmount = discountAmount
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

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
                    _context.OrderItems.Add(orderItem);

                    // Cập nhật tồn kho
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity -= item.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        _context.Inventories.Update(inventory);
                    }
                }

                // Tạo thanh toán
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    Amount = totalAmount - discountAmount,
                    PaymentMethod = paymentMethod,
                    PaymentDate = DateTime.Now
                };
                _context.Payments.Add(payment);

                // Cập nhật trạng thái đơn hàng thành đã thanh toán
                order.Status = "paid";
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
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
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Promotion?> ValidatePromoCodeAsync(string promoCode, decimal orderAmount)
        {
            var promo = await _context.Promotions
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
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null || order.Status != "pending")
                {
                    return false;
                }

                order.Status = "cancelled";
                _context.Orders.Update(order);

                // Hoàn lại tồn kho
                var orderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync();

                foreach (var item in orderItems)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                    if (inventory != null)
                    {
                        inventory.Quantity += item.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        _context.Inventories.Update(inventory);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
