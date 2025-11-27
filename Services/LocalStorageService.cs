using Microsoft.JSInterop;
using System.Text.Json;

namespace CustomerShop.Services
{
    /// <summary>
    /// Service để tương tác với Local Storage thông qua JavaScript Interop
    /// </summary>
    public interface ILocalStorageService
    {
        ValueTask SetItemAsync<T>(string key, T value);
        ValueTask<T?> GetItemAsync<T>(string key);
        ValueTask RemoveItemAsync(string key);
        ValueTask ClearAsync();
    }

    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask SetItemAsync<T>(string key, T value)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                await _jsRuntime.InvokeVoidAsync("localStorageHelper.setItem", key, json);
            }
            catch (InvalidOperationException)
            {
                // Xảy ra khi prerender - bỏ qua
            }
        }

        public async ValueTask<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorageHelper.getItem", key);
                if (string.IsNullOrEmpty(json))
                    return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (InvalidOperationException)
            {
                // Xảy ra khi prerender - trả về default
                return default;
            }
            catch (JsonException)
            {
                return default;
            }
        }

        public async ValueTask RemoveItemAsync(string key)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorageHelper.removeItem", key);
            }
            catch (InvalidOperationException)
            {
                // Xảy ra khi prerender - bỏ qua
            }
        }

        public async ValueTask ClearAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorageHelper.clear");
            }
            catch (InvalidOperationException)
            {
                // Xảy ra khi prerender - bỏ qua
            }
        }
    }
}
