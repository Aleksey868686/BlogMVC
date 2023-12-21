namespace Blog.Models;

/// <summary>
/// Данные для авторизации.
/// </summary>
public class UserCredential
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

    // Внешний ключ
    public Guid UserId { get; set; }
    // Навигационное свойство
    public User User { get; set; }
}
