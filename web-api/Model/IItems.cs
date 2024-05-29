
namespace web_api.Model
{
    public interface IItems
    {
        string Id { get; set; }
        string UserId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        bool Completed { get; set; }
    }
}
