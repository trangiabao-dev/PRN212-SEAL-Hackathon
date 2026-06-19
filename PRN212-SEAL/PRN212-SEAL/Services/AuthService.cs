using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN212_SEAL.Entities;

namespace PRN212_SEAL.Services;

public class AuthService : IAuthService
{
    private readonly PRN212SealDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(PRN212SealDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Account?> LoginAsync(string username, string password)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Username == username);
        if (account == null)
            return null;

        if (_passwordHasher.VerifyPassword(password, account.PasswordHash))
        {
            return account;
        }

        return null;
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string username, string password, string email, string fullName, string role)
    {
        // Business Rule: Check duplicate username
        var existingUser = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Username == username);
        if (existingUser != null)
        {
            return (false, "Username đã tồn tại.");
        }

        // Business Rule: Check duplicate email
        var existingEmail = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        if (existingEmail != null)
        {
            return (false, "Email đã được sử dụng.");
        }

        var newAccount = new Account
        {
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(password),
            Email = email,
            FullName = fullName,
            Role = role,
            CreatedAt = DateTime.Now
        };

        _dbContext.Accounts.Add(newAccount);
        await _dbContext.SaveChangesAsync();

        return (true, string.Empty);
    }
}
