using Data.Entities;

namespace Service.IServices;

public interface IAddressService
{
    Task<IEnumerable<Address>> GetAll();
    Task<Address> GetById(string id);
    Task<Address> Create(string userId, Address model); 
    Task<IEnumerable<Address>> GetByUserId(string userId);
    Task Delete(string id);
    Task<Address> GetDefaultAddressByUserId(string userId); 
}