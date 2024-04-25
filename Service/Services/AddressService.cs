using Common.Constants;
using Common.Exception;
using Data.Entities;
using Data.IRepository;
using Service.IServices;

namespace Service.Services;

public class AddressService : IAddressService
{
    public readonly IRepository<Address> _addressRepo;

    public AddressService(IRepository<Address> addressRepo)
    {
        _addressRepo = addressRepo;
    }

    public async Task<IEnumerable<Address>> GetAll()
    {
        return await _addressRepo.GetAllAsync(a => a.IsDeleted == false);
    }

    public async Task<Address> GetById(string id)
    {
        return await _addressRepo.GetByIdAsync(id);
    }

    public async Task<Address> Create(string userId, Address model)
    {
        var addressDefault = await _addressRepo.GetFirstOrDefaultAsync(a =>
            a.AddressStatus == StringEnum.AddressStatus.Default && a.CreatedBy == userId);
        if (addressDefault == null)
        {
            model.AddressStatus = StringEnum.AddressStatus.Default;
        }

        if (addressDefault != null)
        {
            model.AddressStatus = StringEnum.AddressStatus.NonDefault;
        }

        model.CreateAt = DateTime.UtcNow;
        model.CreatedBy = userId;
        await _addressRepo.AddAsync(model);
        await _addressRepo.SaveChangesAsync();
        return model;
    }

    public async Task<IEnumerable<Address>> GetByUserId(string userId)
    {
        return await _addressRepo.GetAllAsync(a => a.CreatedBy == userId && a.IsDeleted == false,
            a => a.OrderByDescending(o => o.AddressStatus), include => include.User);
    }

    public async Task Delete(string id)
    {
        var address = await _addressRepo.GetByIdAsync(id);
        if (address == null)
            throw new AppException("Address not exit");
        address.IsDeleted = true;
        _addressRepo.Update(address);
        await _addressRepo.SaveChangesAsync();
    }

    public async Task<Address> GetDefaultAddressByUserId(string userId)
    {
       return await _addressRepo.GetFirstOrDefaultAsync(a =>
            a.CreatedBy == userId && a.AddressStatus == StringEnum.AddressStatus.Default, a => a.User); 
    }
}