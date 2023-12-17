namespace Blog.Models;

/// <summary>
/// модель пользователя в блоге
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime JoinDate { get; set; }

    // Navigation properties
    public ICollection<Post> UserPosts { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
