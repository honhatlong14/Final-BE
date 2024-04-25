using Backend.Dtos.ChatDtos;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Webapi.Attributes;

namespace Webapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : BaseApiController
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var res = await _chatService.GetChatByUserId(UserId);
        return Ok(res);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateChatRequest model)
    {
        var res = await _chatService.Create(model.PartnerId, UserId);
        return Ok(res.CreateChatResponseModels);
    }
        
    [HttpGet("messages/{id}/{page:int}")]
    public async Task<IActionResult> GetAll(string id, int page)
    {
        var res = await _chatService.Messages(id, page);
        return Ok(res);
    }
        
    [HttpPost("upload-image")]
    public IActionResult UpLoadImage([FromForm] UploadImageRequest model)
    {
        using var stream = model.image.OpenReadStream();
            
        var res = _chatService.UpLoadImage(stream);
        return Ok(res);
    }
        
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var res = await _chatService.Delete(id);
        return Ok(res);
    }
}