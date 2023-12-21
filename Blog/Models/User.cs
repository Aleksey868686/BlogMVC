namespace Blog.Models;

/// <summary>
/// модель пользователя в блоге
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime JoinDate { get; set; }

    // Навигационное свойство
    public UserCredential UserCredential { get; set; }
    public ICollection<Role> UserRoles { get; set; }
    public ICollection<Post> UserPosts { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
