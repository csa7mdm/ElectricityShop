using System;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;

namespace ElectricityShop.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<User> Users { get; }
        IRepository<Cart> Carts { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Address> Addresses { get; }
        IRepository<ProductAttribute> ProductAttributes { get; }
        IRepository<ProductImage> ProductImages { get; }
        
        Task<int> CompleteAsync();
    }
}