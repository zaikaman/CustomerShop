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
        Task LoadCartFromStorageAsync();
        Task SaveCartToStorageAsync();
        event Action? OnChange;
    }

    /// <summary>
    /// Service quản lý giỏ hàng - lưu trữ vào LocalStorage
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private Cart _cart = new Cart();
        private const string CART_STORAGE_KEY = "customer_cart";
        private bool _isLoaded = false;

        public event Action? OnChange;

        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public Cart GetCart()
        {
            return _cart;
        }

        /// <summary>
        /// Tải giỏ hàng từ LocalStorage
        /// </summary>
        public async Task LoadCartFromStorageAsync()
        {
            if (_isLoaded) return;
            
            try
            {
                var savedItems = await _localStorage.GetItemAsync<List<CartItem>>(CART_STORAGE_KEY);
                if (savedItems != null && savedItems.Count > 0)
                {
                    _cart.Items = savedItems;
                    NotifyStateChanged();
                }
                _isLoaded = true;
            }
            catch
            {
                // Nếu lỗi, giữ giỏ hàng trống
                _isLoaded = true;
            }
        }

        /// <summary>
        /// Lưu giỏ hàng vào LocalStorage
        /// </summary>
        public async Task SaveCartToStorageAsync()
        {
            try
            {
                await _localStorage.SetItemAsync(CART_STORAGE_KEY, _cart.Items);
            }
            catch
            {
                // Bỏ qua lỗi khi lưu
            }
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            _cart.AddItem(product, quantity);
            NotifyStateChanged();
            _ = SaveCartToStorageAsync(); // Fire and forget
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            _cart.UpdateQuantity(productId, quantity);
            NotifyStateChanged();
            _ = SaveCartToStorageAsync(); // Fire and forget
        }

        public void RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId);
            NotifyStateChanged();
            _ = SaveCartToStorageAsync(); // Fire and forget
        }

        public async void ClearCart()
        {
            _cart.Clear();
            NotifyStateChanged();
            await _localStorage.RemoveItemAsync(CART_STORAGE_KEY);
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
