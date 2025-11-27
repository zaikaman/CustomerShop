using System.ComponentModel.DataAnnotations;

namespace CustomerShop.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 999999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 999999999.99)]
        public decimal Subtotal { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
