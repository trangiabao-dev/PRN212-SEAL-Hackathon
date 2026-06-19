using System.Threading.Tasks;
using PRN212_SEAL.Entities;

namespace PRN212_SEAL.Services;

public interface IAuthService
{
    Task<Account?> LoginAsync(string username, string password);
    Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string password, string email, string fullName, string role);
}
