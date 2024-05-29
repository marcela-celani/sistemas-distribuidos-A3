// Interfaces/IUser.cs
namespace web_api.Interfaces
{
    public interface IUser
    {
        string Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
    }
}
