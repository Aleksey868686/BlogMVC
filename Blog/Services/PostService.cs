using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class PostService
{
    private readonly BlogDbContext _context;

    public PostService(BlogDbContext context) => _context = context;    

    public async Task<List<Post>> GetAllPostsAsync() => await _context.Posts.ToListAsync();    

    public async Task<Post> GetPostByIdAsync(Guid id) => await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Post>> GetPostsByUserIdAsync(Guid userId)
    {
        return await _context.Posts
                             .Where(p => p.UserId == userId)
                             .ToListAsync();
    }

    public async Task AddPostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserPostOwnerAsync(Guid userId, Guid postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        return post != null && post.UserId == userId;
    }
}
