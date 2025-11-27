using CustomerShop.Models;

namespace CustomerShop.Services
{
    public interface ICartService
    {
        Cart GetCart();
        void AddToCart(Product product, int quantity = 1);
        void UpdateQuantity(int productId, int quantity);
        void RemoveFromCart(int productId);
        void ClearCart();
        int GetCartItemCount();
        decimal GetCartTotal();
        event Action? OnChange;
    }

    /// <summary>
    /// Service quản lý giỏ hàng - lưu trữ trong memory
    /// Trong thực tế có thể lưu vào ProtectedSessionStorage hoặc LocalStorage
    /// </summary>
    public class CartService : ICartService
    {
        private Cart _cart = new Cart();

        public event Action? OnChange;

        public Cart GetCart()
        {
            return _cart;
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            _cart.AddItem(product, quantity);
            NotifyStateChanged();
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            _cart.UpdateQuantity(productId, quantity);
            NotifyStateChanged();
        }

        public void RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId);
            NotifyStateChanged();
        }

        public void ClearCart()
        {
            _cart.Clear();
            NotifyStateChanged();
        }

        public int GetCartItemCount()
        {
            return _cart.TotalItems;
        }

        public decimal GetCartTotal()
        {
            return _cart.TotalAmount;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
