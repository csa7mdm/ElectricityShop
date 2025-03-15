using System;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using ElectricityShop.Infrastructure.Persistence.Context;
using ElectricityShop.Infrastructure.Persistence.Repositories;

namespace ElectricityShop.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;

        private IRepository<Product> _products;
        private IRepository<Category> _categories;
        private IRepository<User> _users;
        private IRepository<Cart> _carts;
        private IRepository<CartItem> _cartItems;
        private IRepository<Order> _orders;
        private IRepository<OrderItem> _orderItems;
        private IRepository<Address> _addresses;
        private IRepository<ProductAttribute> _productAttributes;
        private IRepository<ProductImage> _productImages;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IRepository<Product> Products => _products ??= new Repository<Product>(_dbContext);
        public IRepository<Category> Categories => _categories ??= new Repository<Category>(_dbContext);
        public IRepository<User> Users => _users ??= new Repository<User>(_dbContext);
        public IRepository<Cart> Carts => _carts ??= new Repository<Cart>(_dbContext);
        public IRepository<CartItem> CartItems => _cartItems ??= new Repository<CartItem>(_dbContext);
        public IRepository<Order> Orders => _orders ??= new Repository<Order>(_dbContext);
        public IRepository<OrderItem> OrderItems => _orderItems ??= new Repository<OrderItem>(_dbContext);
        public IRepository<Address> Addresses => _addresses ??= new Repository<Address>(_dbContext);
        public IRepository<ProductAttribute> ProductAttributes => _productAttributes ??= new Repository<ProductAttribute>(_dbContext);
        public IRepository<ProductImage> ProductImages => _productImages ??= new Repository<ProductImage>(_dbContext);

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }
}