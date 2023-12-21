using Blog.Configs;
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
        // Assign default role (e.g., "User") to the new user
        // Consider hashing the password here if storing passwords

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
}
