using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class TagService
{
    private readonly BlogDbContext _context;

    public TagService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetAllTagsAsync() => await _context.Tags.ToListAsync();

    public async Task<Tag> GetTagByIdAsync(Guid id) => await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
