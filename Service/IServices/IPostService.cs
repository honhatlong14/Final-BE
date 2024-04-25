using Data.Entities;

namespace Service.IServices;

public interface IPostService 
{
    Task<IEnumerable<Post>> GetAll();
    Task<Post> GetById(string id);
    Task<Post> Create(string userId, Post model, string stallId, string bookId);
    Task<Post> Update(string id, string userId, Post model);
    Task Delete(string id);
    Task<IEnumerable<Post>> GetByUserId(string userId);
    Task<Post> GetByBookId(string bookId);
    
}