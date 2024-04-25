using Common.Exception;
using Data.Entities;
using Data.IRepository;
using Service.IServices;

namespace Webapi.Controllers;

public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepo; 
    
    public CommentService(IRepository<Comment> commentRepo)
    {
        _commentRepo = commentRepo; 
    }
    public async Task<IEnumerable<Comment>> GetAll()
    {
        return await _commentRepo.GetAllAsync(null , null, x => x.User);
    }

    public async Task<IEnumerable<Comment>> GetAllByPostId(string postId)
    {
        return await _commentRepo.GetAllAsync(x => x.PostId == postId , null, x => x.User);
    }

    public async Task<Comment> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Comment> Create(Comment model)
    {
        var commentExist =
            await _commentRepo.GetFirstOrDefaultAsync(x => x.UserId == model.UserId && x.PostId == model.PostId);

        if (commentExist != null)
        {
            throw new AppException("User had commented!"); 
        }
        await _commentRepo.AddAsync(model);
        await _commentRepo.SaveChangesAsync();
        return model;
    }

    public async Task<IEnumerable<Comment>> GetByUserId(string userId)
    {
        return await _commentRepo.GetAllAsync(x => x.UserId == userId);
    }

    public async Task Delete(string id)
    {
        throw new NotImplementedException();
    }
}