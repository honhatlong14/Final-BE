using AutoMapper;
using Data.Entities;
using Data.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;




[ApiController]
[Route("api/[controller]")]
public class CommentController : BaseApiController
{
    private readonly ICommentService _commentService;
    private readonly IBookService _bookService; 
    private readonly IMapper _mapper;
    
    public CommentController(ICommentService commentService, IMapper mapper, IBookService bookService)
    {
        _commentService = commentService;
        _mapper = mapper;
        _bookService = bookService; 
    }
    
    [HttpGet]
    public async Task<IEnumerable<Comment>> GetAll()
    {
        var comments = await _commentService.GetAll();
        return comments; 
    }
    
    [HttpGet("getAllByPostId/{postId}")]
    public async Task<IEnumerable<Comment>> GetAllByPostId(string postId)
    {
        var comments = await _commentService.GetAllByPostId(postId);
        return comments; 
    }
    
    [HttpGet("{userId}")]
    public async Task<IEnumerable<Comment>> GetByUserId(string userId)
    {
        var comments = await _commentService.GetByUserId(userId);
        return comments; 
    }
    
    [HttpPost]
    public async  Task<ActionResult<Comment>> Create([FromBody] CreateCommentRequest model)
    {
        var comment = _mapper.Map<Comment>(model);
        await _commentService.Create(comment);
        await _bookService.CalculateRating(model.BookId, model.Rating); 
        return Ok("Create Comment Successfully");
    }
}