using Microsoft.EntityFrameworkCore;
using CustomerShop.Data;
using CustomerShop.Models;

namespace CustomerShop.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> SearchProductsAsync(string searchTerm);
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Product>> GetProductsWithFiltersAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize);
        Task<int> GetTotalProductsCountAsync(int? categoryId, string? searchTerm);
        Task<(List<Product> Products, int TotalCount, List<Category> Categories)> GetShopDataAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize);
    }

    public class ProductService : IProductService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ProductService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == categoryId && p.Inventory != null && p.Inventory.Quantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var lowerSearchTerm = searchTerm.ToLower();
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => (p.ProductName.ToLower().Contains(lowerSearchTerm) || 
                            (p.Category != null && p.Category.CategoryName.ToLower().Contains(lowerSearchTerm))) &&
                            p.Inventory != null && p.Inventory.Quantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsWithFiltersAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                .AsQueryable();

            // Filter by category
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(lowerSearchTerm) ||
                                        (p.Category != null && p.Category.CategoryName.ToLower().Contains(lowerSearchTerm)));
            }

            // Sort
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Pagination
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductsCountAsync(int? categoryId, string? searchTerm)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Products
                .Include(p => p.Inventory)
                .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(lowerSearchTerm) ||
                                        context.Categories.Any(c => c.CategoryId == p.CategoryId && c.CategoryName.ToLower().Contains(lowerSearchTerm)));
            }

            return await query.CountAsync();
        }

        public async Task<(List<Product> Products, int TotalCount, List<Category> Categories)> GetShopDataAsync(
            int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize)
        {
            // Chạy 3 query song song với 3 DbContext riêng biệt
            var categoriesTask = Task.Run(async () =>
            {
                await using var ctx = await _contextFactory.CreateDbContextAsync();
                return await ctx.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            });

            var countTask = Task.Run(async () =>
            {
                await using var ctx = await _contextFactory.CreateDbContextAsync();
                var query = ctx.Products
                    .Include(p => p.Inventory)
                    .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                    .AsQueryable();

                if (categoryId.HasValue && categoryId > 0)
                    query = query.Where(p => p.CategoryId == categoryId);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(lowerSearchTerm) ||
                                            ctx.Categories.Any(c => c.CategoryId == p.CategoryId && c.CategoryName.ToLower().Contains(lowerSearchTerm)));
                }

                return await query.CountAsync();
            });

            var productsTask = Task.Run(async () =>
            {
                await using var ctx = await _contextFactory.CreateDbContextAsync();
                var query = ctx.Products
                    .Include(p => p.Category)
                    .Include(p => p.Inventory)
                    .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                    .AsQueryable();

                if (categoryId.HasValue && categoryId > 0)
                    query = query.Where(p => p.CategoryId == categoryId);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(p => p.ProductName.ToLower().Contains(lowerSearchTerm) ||
                                            (p.Category != null && p.Category.CategoryName.ToLower().Contains(lowerSearchTerm)));
                }

                query = sortBy switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "name_asc" => query.OrderBy(p => p.ProductName),
                    "name_desc" => query.OrderByDescending(p => p.ProductName),
                    _ => query.OrderByDescending(p => p.CreatedAt)
                };

                return await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            });

            // Đợi tất cả cùng lúc
            await Task.WhenAll(categoriesTask, countTask, productsTask);

            return (productsTask.Result, countTask.Result, categoriesTask.Result);
        }
    }
}
