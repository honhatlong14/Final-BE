    using Common.Exception;
using Data.Entities;
using Data.Entities.User;
using Data.IRepository;
using Service.IServices;

namespace Service.Services;

public class PostService : IPostService
{
    public readonly IRepository<Post> _postRepo;
    public PostService(IRepository<Post> postRepo, IRepository<Stall> stallRepo)
    {
        _postRepo = postRepo;
    }
    public async Task<IEnumerable<Post>> GetAll()
    {
        return await _postRepo.GetAllAsync(); 
    }

    public async Task<Post> GetById(string id)
    {
        return await _postRepo.GetByIdAsync(id); 
    }

    public async Task<Post> Create(string userId, Post model, string stallId, string bookId)
    {
        if((await _postRepo.GetFirstOrDefaultAsync(p => p.StallId == stallId && p.BookId == bookId)) != null)
            throw new AppException("Post is already exists"); 
        model.StallId = stallId;
        model.BookId = bookId; 
        model.CreateAt = DateTime.UtcNow;
        model.CreatedBy = userId;
        await _postRepo.AddAsync(model);
        await _postRepo.SaveChangesAsync();
        return model;
    }

    public Task<Post> Update(string id, string userId, Post model)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Post>> GetByUserId(string userId)
    {
        return await _postRepo.GetAllAsync(p => p.CreatedBy == userId, null ,include => include.Book.Images ); 
    }

    public async Task<Post> GetByBookId(string bookId)
        {
        
        var post = await _postRepo.GetFirstOrDefaultAsync(p => p.BookId == bookId, p => p.Stall);
        return post; 
    }
}