namespace Blog.Models;

/// <summary>
/// Модель комментария к посту в блоге
/// </summary>
public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

    // Navigation property
    public Post Post { get; set; }
    public User User { get; set; }
}
