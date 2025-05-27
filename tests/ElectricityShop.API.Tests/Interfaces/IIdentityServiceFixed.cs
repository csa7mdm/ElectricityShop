using System.Threading.Tasks;
using ElectricityShop.Application.Common.Models;

namespace ElectricityShop.Application.Common.Interfaces
{
    // This is a fixed version of the interface with proper Result type
    public interface IIdentityServiceFixed
    {
        Task<Result> RegisterAsync(string email, string password, string firstName, string lastName);
        Task<Result> LoginAsync(string email, string password);
        Task<Result> RefreshTokenAsync(string token, string refreshToken);
    }
}