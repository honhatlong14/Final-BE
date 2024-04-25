using Data.Entities;

namespace Service.IServices;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetAll();
    Task<IEnumerable<Comment>> GetAllByPostId(string postId);
    Task<Comment> GetById(string id);
    Task<Comment> Create(Comment model);
    Task<IEnumerable<Comment>> GetByUserId(string userId);
    Task Delete(string id);
}