using Common.Exception;
using Data.Entities;
using Data.IRepository;
using Service.IServices;
using Stripe;
using StringEnum = Common.Constants.StringEnum;

namespace Service.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepo; 
    public CategoryService(IRepository<Category> categoryRepo)
    {
        _categoryRepo = categoryRepo; 
    }
    
    public async Task<IEnumerable<Category>> GetAllActivate()
    {
        return await _categoryRepo.GetAllAsync(x => x.CategoryStatus == StringEnum.CategoryStatus.Activate, null, null); 
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        return await _categoryRepo.GetAllAsync(); 
    }

    public async Task<Category> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> Create(string name)
    {
        var category = new Category();
        category.CategoryName = name;
        await _categoryRepo.AddAsync(category);
        await _categoryRepo.SaveChangesAsync();
        return category;
    }

    public async Task Delete(string id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
            throw new AppException("Category not exit");
         _categoryRepo.Remove(category);
        await _categoryRepo.SaveChangesAsync();
    }
}