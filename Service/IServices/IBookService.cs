using Data.Entities;
using Data.Entities.User;
using Data.ViewModels.Book;

namespace Service.IServices;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAll();
    Task<Book> GetById(string id);
    Task<IEnumerable<Book>> GetByCategoryId(string categoryId);
    Task<string> ProfitBookBySold(string stallId);
    Task<Book> Create(Book model, string userId);
    Task CalculateRating(string bookId, int rating);
    Task<IEnumerable<Book>> GetByRating(decimal rating);
    Task<Book> Update(string id, User model);
    Task Delete(string id);
    Task<Post> CreateBookPost(string userId, Book book, Post post, IList<Category> categories);
    
    Task<Post> UpdateBookPost(string userId, string bookId, Book book, Post post, IList<Category> categories);

    Task<IEnumerable<Book>> GetByUserId(string userId);
    Task<IEnumerable<Book>> GetBookBySort(string sort);
    Task<string> GetProfitBookQuality();
    // Task<Cart> AddToCart(string bookId, string userId, int quantity);
}