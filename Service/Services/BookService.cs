using Common.Constants;
using Common.Exception;
using Data.Entities;
using Data.Entities.User;
using Data.IRepository;
using Data.ViewModels.Book;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using Service.IServices;

namespace Service.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<Cart> _cartRepo;
    private readonly IRepository<BookCategory> _bookCateRepo;


    public BookService(IRepository<Book> bookRepo, IRepository<Post> postRepo, IRepository<Cart> cartRepo, IRepository<BookCategory> bookCateRepo)
    {
        _bookRepo = bookRepo;
        _postRepo = postRepo;
        _cartRepo = cartRepo;
        _bookCateRepo = bookCateRepo; 
    }
    
    public async Task<IEnumerable<Book>> GetAll()
    {
        return await _bookRepo.GetAllAsync(); 
    }
    
    public async Task<IEnumerable<Book>> GetByRating(decimal rating)
    {
        if(rating == 5)
            return await _bookRepo.GetAllAsync(x => x.Rating == 5 && x.Post.SellStatus == StringEnum.SellStatus.Available, null , x => x.Images); 
        if(rating >= 4) 
            return await _bookRepo.GetAllAsync(x => x.Rating >=4 && x.Rating <= 5 && x.Post.SellStatus == StringEnum.SellStatus.Available , null , x => x.Images);
        if(rating >= 3)
            return await _bookRepo.GetAllAsync(x => x.Rating >=3 && x.Rating <= 4 && x.Post.SellStatus == StringEnum.SellStatus.Available , null , x => x.Images);
        
        return null; 
    }

  

    public async Task<Book> GetById(string id)
    {
        return await _bookRepo.GetFirstOrDefaultWithCategoriesAsync(
            e => e.Id == id, b => b.Images, b => b.BookCategories, b => b.Post);
    }
    
    public async Task<IEnumerable<Book>> GetByCategoryId(string categoryId)
    {
        var books = (await _bookCateRepo.GetAllAsync(x => x.CategoryId == categoryId && x.Book.Post.SellStatus == StringEnum.SellStatus.Available, null, c => c.Book, c => c.Book.Images)).Select(c => c.Book);
        if (books == null) throw new AppException("Filter by Category not found"); 
        return books; 
    }

    public async Task<string> ProfitBookBySold(string stallId)
    {
        throw new NotImplementedException();
    }

    public async Task<Book> Create(Book model, string userId)
    {
        model.Rating = 5; 
        model.CreateAt = DateTime.UtcNow;
        model.CreatedBy = userId;
        await _bookRepo.AddAsync(model);
        await _bookRepo.SaveChangesAsync();
        return model;

    }

    public async Task CalculateRating(string bookId, int rating)
    {
        var book = await _bookRepo.GetFirstOrDefaultAsync(x => x.Id == bookId);
        book.Rating = (book.Rating + rating) / 2; 
        _bookRepo.Update(book);
        await _bookRepo.SaveChangesAsync();
    }

    public Task<Book> Update(string id, User model)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }

  

    public async Task<Post> CreateBookPost(string userId, Book book, Post post, IList<Category> categories)
    {
        // await Create(book, userId);
        book.Rating = 5;
        book.CreateAt = DateTime.UtcNow;
        book.CreatedBy = userId; 
        await _bookRepo.AddAsync(book);
        List<BookCategory> bookCate = new List<BookCategory>();

        foreach (var category in categories)
        {
            var bookCategory = new BookCategory()
            {
                BookId = book.Id,
                CategoryId = category.Id
            };
            // book.BookCategories.Add(bookCategory);
     
            bookCate.Add(bookCategory);
        }
        

        post.BookId = book.Id;
        post.CreatedBy = userId; 
        
        await _bookCateRepo.AddRangeAsync(bookCate);
        await _postRepo.AddAsync(post);
        await _postRepo.SaveChangesAsync();
        await _bookCateRepo.SaveChangesAsync();

        return post; 
    }

    public async Task<Post> UpdateBookPost(string userId, string bookId, Book book, Post post,
        IList<Category> categories)
    {
        var bookUpdate =
            await _bookRepo.GetFirstOrDefaultAsync(b => b.Id == bookId, b => b.Post, b => b.BookCategories);

        if (bookUpdate == null)
        {
            // Xử lý trường hợp không tìm thấy Book cần cập nhật
            // Điều này phụ thuộc vào logic của ứng dụng bạn
            return null;
        }

        // Cập nhật thông tin cơ bản của Book
        bookUpdate.Title = book.Title;
        bookUpdate.Description = book.Description;
        bookUpdate.NumPage = book.NumPage;
        bookUpdate.AvailbleQuantity = book.AvailbleQuantity;
        bookUpdate.Author = book.Author;
        bookUpdate.Price = book.Price;
        bookUpdate.Images = book.Images; 

        // Lấy danh sách CategoryId từ List<Category>
        var categoryIds = categories.Select(c => c.Id).ToList();

        // Lấy danh sách BookCategoryId hiện tại của Book
        var currentBookCategoryIds = bookUpdate.BookCategories.Select(bc => bc.CategoryId).ToList();

        // Duyệt qua danh sách BookCategoryId hiện tại của Book
        foreach (var currentCategoryId in currentBookCategoryIds)
        {
            // Nếu CategoryId hiện tại không có trong danh sách mới, xóa nó đi
            if (!categoryIds.Contains(currentCategoryId))
            {
                var categoryToRemove =
                    bookUpdate.BookCategories.FirstOrDefault(bc => bc.CategoryId == currentCategoryId);
                bookUpdate.BookCategories.Remove(categoryToRemove);
            }
        }

        // Duyệt qua danh sách mới
        foreach (var categoryId in categoryIds)
        {
            // Nếu CategoryId không có trong danh sách hiện tại, thêm nó vào
            if (!currentBookCategoryIds.Contains(categoryId))
            {
                var newBookCategory = new BookCategory
                {
                    BookId = bookUpdate.Id,
                    CategoryId = categoryId
                };
                bookUpdate.BookCategories.Add(newBookCategory);
            }
        }

        // Cập nhật thông tin của Post (giả sử có thông tin cập nhật cho Post)
        if (post != null)
        {
            bookUpdate.Post = post;
            post.CreatedBy = userId; 
        }

        // Lưu các thay đổi vào cơ sở dữ liệu
        _bookRepo.Update(bookUpdate);
        await _bookRepo.SaveChangesAsync();

        return bookUpdate.Post;
    }




    public async Task<IEnumerable<Book>> GetByUserId(string userId)
    {
        return await _bookRepo.GetAllAsync(b => b.CreatedBy == userId); 
    }

    public async Task<IEnumerable<Book>> GetBookBySort(string sort)
    {
        if (sort == "newest")
        {
            return await _bookRepo.GetAllAsync(b => b.Post.SellStatus == StringEnum.SellStatus.Available , b => b.OrderByDescending(b => b.CreateAt), b => b.Images);
        }
        if (sort == "highestPrice")
        {
            return await _bookRepo.GetAllAsync(b => b.Post.SellStatus == StringEnum.SellStatus.Available , b => b.OrderByDescending(b => b.Price) , b => b.Images);
        }
        if (sort == "lowestPrice")
        {
            return await _bookRepo.GetAllAsync(b => b.Post.SellStatus == StringEnum.SellStatus.Available , b => b.OrderBy(b => b.Price) , b => b.Images);
        }
        if (sort == "bestSellers")
        {
            return await _bookRepo.GetAllAsync(b => b.Post.SellStatus == StringEnum.SellStatus.Available , b => b.OrderBy(b => b.QuantitySold), b => b.Images );
        }

        return null; 
    }

    public async Task<string> GetProfitBookQuality()
    {
        var usersCounting = (await _bookRepo.GetAllAsync(x => x.Rating >=(decimal)4.8)).Count();
        var jsonData = JsonConvert.SerializeObject(usersCounting, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );

        return jsonData;
    }
}