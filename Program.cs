using Microsoft.EntityFrameworkCore;
using CustomerShop.Components;
using CustomerShop.Data;
using CustomerShop.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load biến môi trường từ file .env
Env.Load();

// Lấy thông tin kết nối từ biến môi trường
var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var port = Environment.GetEnvironmentVariable("DATABASE_PORT");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

var connectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";

// Cấu hình DbContext với MySQL - dùng Pooled DbContextFactory cho Blazor Server
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Đăng ký LocalStorageService để lưu trữ dữ liệu trên browser
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Đăng ký Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// CartService cần là Scoped để mỗi user có giỏ hàng riêng
builder.Services.AddScoped<ICartService, CartService>();

// ToastService cho thông báo
builder.Services.AddScoped<IToastService, ToastService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
