using AutoMapper;
using Common.Exception;
using Data.Entities;
using Data.IRepository;
using Data.ViewModels.Post;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Service.Services;

namespace Webapi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PostController : BaseApiController
{
    private readonly IPostService _postService; 
    private readonly IMapper _mapperService; 

    public PostController(IPostService postService, IMapper mapperService)
    {
        _postService = postService;
        _mapperService = mapperService; 
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> GetAll()
    {
        var result = await _postService.GetAll();
        return result; 
    }
    
    [HttpGet("id")]
    public async Task<ActionResult<Post>> GetById(string id)
    {
        var post = await _postService.GetById(id);
        return Ok(post);
    }
    
    [HttpGet("getByUserId/{userId}")]
    public async Task<ActionResult<Post>> GetByUserId(string userId)
    {
        var post = await _postService.GetByUserId(userId);
        return Ok(post);
    }
    
    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromForm]PostRegisterRequest model)
    {
        var post = _mapperService.Map<Post>(model);
        await _postService.Create(UserId, post, model.StallId, model.BookId);   
        return Ok("Create Post Successful");
    }
    
    [HttpGet("getByBookId/{bookId}")]
    public async Task<Post> GetByBookId(string bookId)
    {
        return await _postService.GetByBookId(bookId);
    }  

}