using Data.Entities;
using Data.ViewModels.Cart;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Service.IServices;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetAll();
    Task<Cart> GetById(string id);
    Task<Cart> Create(string userId, Cart model, string stallId, string bookId);
    Task<Cart> Update(string id, string userId, Cart model);
    Task Delete(string id);
    Task<IEnumerable<Cart>> GetByUserId(string userId);
    Task<Cart> AddToCart(string bookId, string userId, int quantity, int total);
    Task<IEnumerable<Cart>> GetAllByUserId(string userId);
    Task<Cart> ReduceQuantity(string id, string userId); 
    Task<Cart> IncrementQuantity(string id, string userId);
    Task<Cart> RemoveByUserId(string id, string userId);
    Task<IEnumerable<Cart>> GetAllCartSelected(string userId);
    Task<IEnumerable<Cart>> RemoveListCartItem(string userId); 
}