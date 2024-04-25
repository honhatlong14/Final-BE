using AutoMapper;
using Common.Exception;
using Data.Entities;
using Data.Entities.User;
using Data.ViewModels.Book;
using Data.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Service.Services;

namespace Webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : BaseApiController
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;
    private readonly IImageService _image; 
    
    public BookController(IBookService bookService, IMapper mapper, IImageService image)
    {
        _bookService = bookService;
        _mapper = mapper;
        _image = image; 
    }

    [HttpGet]
    public async Task<IEnumerable<Book>> GetAll()  
    {
        var books = await _bookService.GetAll();
        return books; 
    }

    [HttpGet("getByRating")]
    public async Task<IEnumerable<Book>> GetByRating([FromQuery]decimal rating)
    {
        return await _bookService.GetByRating(rating); 
    }
    

    [HttpGet("getByCategoryId/{categoryId}")]
    public async Task<IEnumerable<Book>> GetByCategoryId(string categoryId)
    {
        var books = await _bookService.GetByCategoryId(categoryId); 
        return books; 
    }
    
    [HttpGet("getByUserId/{userId}")]
    public async Task<ActionResult<Book>> GetByUserId(string userId)
    {
        var books = await _bookService.GetByUserId(userId);
        return Ok(books);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetById(string id)
    {
        var book = await _bookService.GetById(id);
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create([FromBody] CreateBookRequest model)
    {
        var listImages = new List<Image>(); 
        var book = _mapper.Map<Book>(model);
        var uploadResult = await _image.UploadImageBase64(model.BookImages);

        foreach (var item in uploadResult)
        {
            var image = new Image
            {
                BookId = book.Id,
                ImageUrl = item.Url.ToString()
            }; 
            listImages.Add(image);  
        }

        book.Images = listImages; 
        return await _bookService.Create(book, UserId);
     }  


    [HttpPost("createBookPost")]
    public async Task<ActionResult<CreateBookPostResponse>> CreateBookPost([FromBody]CreateBookPostRequset model)
    {
        // ở đây: tạo book + post rồi map tuừ model vào. ở service thì chỉ cần gắn vào là xong 
        var book = _mapper.Map<Book>(model);
        
        await _image.UploadImageBase64(model.BookImages, book); 
        var post = _mapper.Map<Post>(model);
        var bookPost = await _bookService.CreateBookPost(UserId, book, post, model.Categories); 
        return Ok(bookPost);    
    }
    
    [HttpPut("updateBookPost/{bookId}")]
    public async Task<ActionResult<CreateBookPostResponse>> UpdateBookPost([FromBody]UpdateBookPostRequest model, string bookId)
    {
        // ở đây: tạo book + post rồi map tuừ model vào. ở service thì chỉ cần gắn vào là xong 
        var book = _mapper.Map<Book>(model);
        
        await _image.UploadImageBase64(model.BookImages, book, bookId); 
        var post = _mapper.Map<Post>(model);
        var bookPost = await _bookService.UpdateBookPost(UserId, bookId, book, post, model.Categories); 
        return Ok(bookPost);    
    }
    
    [HttpGet("getBookBySort")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBookBySort([FromQuery]string sort)
    {
        var booksSort = await _bookService.GetBookBySort(sort); 
        return Ok(booksSort);
    }
    
    [AllowAnonymous]
    [HttpGet("getProfitBookQuality")]
    public async Task<ActionResult<string>> GetProfitBookQuality()
    {
        var profit = await _bookService.GetProfitBookQuality();
        return Ok(profit); 
    }
    
   
}