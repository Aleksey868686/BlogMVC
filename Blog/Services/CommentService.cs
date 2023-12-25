using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class CommentService
{
    private readonly BlogDbContext _context;

    public CommentService(BlogDbContext context) => _context = context;
    
    public async Task<List<Comment>> GetAllCommentsAsync() => await _context.Comments.ToListAsync();
    
    public async Task<Comment> GetCommentByIdAsync(Guid id) => await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
    
    public async Task AddCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserCommentOwnerAsync(Guid userId, Guid commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        return comment != null && comment.UserId == userId;
    }
}
