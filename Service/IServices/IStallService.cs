using Common.Constants;
using Data.Entities.User;

namespace Service.IServices;

public interface IStallService
{
    Task<IEnumerable<Stall>> GetAll();
    Task<Stall> GetById(string id);
    Task<Stall> GetByUserId(string userId);
    Task<Stall> Create(string userId, Stall model);
    Task<Stall> Update(string id, string userId, Stall model);
    Task Delete(string id);
    Task UpdateStallStatus(StringEnum.StallStatus stallStatus, string stallId);
}