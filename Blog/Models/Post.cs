namespace Blog.Models;

/// <summary>
///  Модель поста в блоге
/// </summary>
public class Post
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime DateCreated { get; set; }
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; }
    public ICollection<PostTag> PostTags { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
