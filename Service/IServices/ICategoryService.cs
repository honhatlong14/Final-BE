using Data.Entities;

namespace Service.IServices;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllActivate();
    Task<IEnumerable<Category>> GetAll();
    Task<Category> GetById(string id);
    Task<Category> Create(string name);
    Task Delete(string id); 
}