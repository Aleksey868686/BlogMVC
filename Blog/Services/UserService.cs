using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Services;

public class UserService
{
    private readonly BlogDbContext _context;

    public UserService(BlogDbContext context) => _context = context;

    public async Task<List<User>> GetAllUsersAsync() => await _context.Users.ToListAsync();
    

    public async Task<User> GetUserByIdAsync(Guid userId) => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    

    public async Task AddUserAsync(User user)
    {
        // Check if the "User" role exists, and if not, add it
        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole == null)
        {
            userRole = new Role { Name = "User" };
            _context.Roles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        // Assign the "User" role to the new user
        if (user.UserRoles == null)
        {
            user.UserRoles = new List<Role>();
        }
        user.UserRoles.Add(userRole);

        // Hash the user's password
        user.UserCredential.Password = HashPassword(user.UserCredential.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User> AuthenticateUserAsync(string login, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserCredential)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserCredential.Login == login);

        if (user != null)
        {
            var verificationResult = VerifyHashedPassword(user.UserCredential.Password, password);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                return user;
            }
        }

        return null;
    }

    private string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.HashPassword(null, password);
    }

    private PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
    }
}


