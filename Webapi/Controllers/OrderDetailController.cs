using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderDetailController : BaseApiController
{
    private readonly IOrderDetailService _orderDetailService; 
    public OrderDetailController(IOrderDetailService orderDetailService)
    {
        _orderDetailService = orderDetailService; 
    }
    
    [HttpGet("getByOrderIdStallId")]
    public async Task<ActionResult<string>> GetByOrderIdStallId([FromQuery]string orderId,[FromQuery] string stallId)
    {
        var data = await _orderDetailService.GetByOrderIdStallId(orderId, stallId); 
        return Ok(data);
    }
    
    [HttpGet("getByOrderId")]
    public async Task<ActionResult<string>> GetByOrderId([FromQuery]string orderId)
    {
        var data = await _orderDetailService.GetByOrderId(orderId); 
        return Ok(data);
    }
    
    [HttpGet("getProfitTotalIncome")]
    public async Task<ActionResult<string>> GetProfitTotalIncome()
    {
        var data = await _orderDetailService.GetProfitTotalIncome(); 
        return Ok(data);
    }
    
    [HttpGet("getTopUserStall/{stallId}")]
    public async Task<ActionResult<string>> GetTopUserStall(string stallId)
    {
        var data = await _orderDetailService.GetTopUserStall(stallId); 
        return Ok(data);
    }
    
    [HttpGet("getTopUser")]
    public async Task<ActionResult<string>> GetTopUser()
    {
        var data = await _orderDetailService.GetTopUser(); 
        return Ok(data);
    }
    
   
}