

using AutoMapper;
using Common.Constants;
using Common.Exception;
using Data.Entities.User;
using Data.ViewModels.Stall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Service.IServices;

namespace Webapi.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class StallController : BaseApiController
{
    private readonly IStallService _stallService; 
    private readonly IMapper _mapperService; 

    public StallController(IStallService stallService, IMapper mapperService)
    {
        _stallService = stallService;
        _mapperService = mapperService; 
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StallsResponse>>> GetAll()
    {
        var stalls = await _stallService.GetAll();
        var stallsMappingResponse = _mapperService.Map<IEnumerable<StallsResponse>>(stalls); 
        
        return Ok(stallsMappingResponse); 
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Stall>> GetById(string id)
    {
        var stall = await _stallService.GetById(id);
        return Ok(stall);
    }
    
    [HttpGet("getByUserId/{userId}")]
    public async Task<ActionResult<Stall>> GetByUserId(string userId)
    {
        var result = await _stallService.GetByUserId(userId);
        return Ok(result);
    }   
    
    
    [HttpPost]
    public async Task<ActionResult<Stall>> Create([FromForm]StallRegisterRequest model)
    {
        var stall = _mapperService.Map<Stall>(model);
        if(string.IsNullOrEmpty(model.StallName)) throw new AppException("Stall Name can not empty");
        await _stallService.Create(UserId, stall);
        return Ok(new { message = "Create Stall Successful" }); 
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Stall>> Update(string id,[FromBody] UpdateStallRequest model)
    {
        var stallUpdate = _mapperService.Map<Stall>(model);
        await _stallService.Update(id, UserId, stallUpdate);
        return Ok(stallUpdate); 
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Stall>> Delete(string id)
    {
        await _stallService.Delete(id);
        return Ok("delete success"); 
    }
    
    [HttpPost("updateStallStatus/{stallId}")]
    public async Task<ActionResult<Stall>> UpdateStallStatus(string stallId, [FromQuery] StringEnum.StallStatus stallStatus)
    {
        await _stallService.UpdateStallStatus(stallStatus, stallId); 
        return Ok("Update success"); 
    }
}