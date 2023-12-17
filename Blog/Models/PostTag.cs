namespace Blog.Models;

/// <summary>
/// Модель для связи постов и тегов в блоге
/// </summary>
public class PostTag
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public Guid PostId { get; set; }

    // Navigation properties
    public Post Post { get; set; }
    public Tag Tag { get; set; }
}
