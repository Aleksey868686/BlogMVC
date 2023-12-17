namespace Blog.Models;

/// <summary>
/// Модель тега для поста в блоге
/// </summary>
public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public ICollection<PostTag> PostTags { get; set; }
}
