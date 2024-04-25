using AutoMapper;
using Common.Constants;
using Common.Exception;
using Data.Entities.User;
using Data.IRepository;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Service.Services;

public class StallService : IStallService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Stall> _stallRepo;
    private readonly IMapper _mapperService; 
    
    public StallService(IRepository<User> userRepo, IRepository<Stall> stallRepo, IMapper mapperService)
    {
        _userRepo = userRepo;
        _stallRepo = stallRepo;
        _mapperService = mapperService; 
    }
    
    public Task<IEnumerable<Stall>> GetAll()
    {
        return _stallRepo.GetAllAsync(null, null, include => include.User); 
    }
  
    public async Task<Stall> GetById(string id)
    {
        var stall = await _stallRepo.GetFirstOrDefaultAsync(x => x.Id == id);
        return stall; 
    }

    public async Task<Stall> GetByUserId(string userId)
    {
        var user = await _userRepo.GetFirstOrDefaultAsync(x => x.Id == userId, include => include.Stall);
        return user.Stall;
    }

    public async Task<Stall> Create(string userId, Stall model)
    {
        // Check stall register yet 
        if (await GetByUserId(userId) != null)
            throw new AppException("Stall has been register. Can not Create!");
        
        // check Stall name has been register 
        if( (await _userRepo.GetFirstOrDefaultAsync(x => x.Stall.StallName == model.StallName)) != null)
            throw new AppException("Stall name has been register. Please choose another name");

        model.CreateAt = DateTime.UtcNow;
        model.StallStatus = StringEnum.StallStatus.Pending; 
        model.UserId = userId; 
        await _stallRepo.AddAsync(model);
        await _stallRepo.SaveChangesAsync();
        return model;
    }

    public async Task<Stall> Update(string id, string userId, Stall model)
    {
        var stall = await _stallRepo.GetByIdAsync(id); 
        var user = await _userRepo.GetFirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new AppException("User not found");
        }

        stall.StallName = model.StallName; 
        model.UpdateAt = DateTime.UtcNow;
        model.UpdateBy = userId;
        
        _stallRepo.Update(stall);
        await _stallRepo.SaveChangesAsync();
        return model; 
    } 

    public async Task Delete(string id)
    {
        var stall = await _stallRepo.GetByIdAsync(id);
        if (stall == null)
            throw new AppException("Stall not exit"); 
        _stallRepo.Remove(stall);
        await _stallRepo.SaveChangesAsync(); 
    }

    public async Task UpdateStallStatus(StringEnum.StallStatus stallStatus, string stallId)
    {
        var stall = await _stallRepo.GetFirstOrDefaultAsync(x => x.Id == stallId);

        if (stallStatus == StringEnum.StallStatus.Deny)
        {
            stall.StallStatus = StringEnum.StallStatus.Deny; 
            _stallRepo.Update(stall);
        }
        if (stallStatus == StringEnum.StallStatus.Activate)
        {
            stall.StallStatus = StringEnum.StallStatus.Activate; 
            _stallRepo.Update(stall);
        }
        if (stallStatus == StringEnum.StallStatus.Lock)
        {
            stall.StallStatus = StringEnum.StallStatus.Lock; 
            _stallRepo.Update(stall);
        }

        await _stallRepo.SaveChangesAsync(); 
    }
}