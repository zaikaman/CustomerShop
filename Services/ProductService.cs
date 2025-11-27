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
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.Inventory != null && p.Inventory.Quantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == categoryId && p.Inventory != null && p.Inventory.Quantity > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Products
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
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsWithFiltersAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize)
        {
            var query = _context.Products
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
            var query = _context.Products
                .Include(p => p.Category)
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
                                        (p.Category != null && p.Category.CategoryName.ToLower().Contains(lowerSearchTerm)));
            }

            return await query.CountAsync();
        }
    }
}
