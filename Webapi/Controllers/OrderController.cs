using AutoMapper;
using Data.Entities;
using Data.ViewModels.OrderDetail;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : BaseApiController
{
    private readonly IOrderService _orderService;

    private readonly IMapper _mapper;
    private readonly IImageService _image;
    private readonly IOrderDetailService _orderDetailService;


    public OrderController(IOrderService orderService, IMapper mapper, IImageService image, IOrderDetailService orderDetailService)
    {
        _mapper = mapper;
        _image = image;
        _orderService = orderService;
        _orderDetailService = orderDetailService; 
    }

    [HttpPost("createOrderCashPayment/{userId}")]
    public async Task<ActionResult<Order>> CreateOrderCashPayment(string userId)
    {
        var order = await _orderService.CreateOrderCashPayment(userId);
        return order;
    }
    
    [HttpPost("updateShippingStatus/{id}/{userId}")]
    public async Task<ActionResult<Order>> UpdateShippingStatus(string id, string userId, [FromQuery]int shippingStatus)
    {
        var order = await _orderService.UpdateShippingStatus(id, userId,shippingStatus);
        return Ok(order);
    }
    
    [HttpGet("getAllByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<GetOrderDetailsResponse>>> GetAllByUserId(string userId)
    {
        var orders = await _orderService.GetAllByUserId(userId);
        return Ok(orders);
    }
    [HttpGet("getOrdersByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<GetOrderResponse>>> GetOrdersByUserId(string userId)
    {
        var orders = await _orderService.GetOrdersByUserId(userId);
        return Ok(orders);
    }
    
    [HttpGet("getOrdersByStallId/{stallId}")]
    public async Task<ActionResult<IEnumerable<GetOrderResponse>>> GetOrdersByStallId(string stallId, [FromQuery]string filter)
    {
        return Ok(await _orderService.GetOrdersByStallId(stallId, filter));
    }
    
    [HttpGet("getProfitByStallId/{stallId}")]
    public async Task<ActionResult<string>> GetProfitByStallId(string stallId)
    {
        
        return Ok(await _orderDetailService.GetProfit(stallId)); 
    }
    
    [HttpGet("getProfitByQuantity/{stallId}")]
    public async Task<ActionResult<string>> GetProfitByQuantity(string stallId)
    {
        
        return Ok(await _orderDetailService.GetProfitByQuantity(stallId)); 
    }

    [HttpGet("getTotalOrder")]
    public async Task<ActionResult<string>> GetTotalOrder()
    {
        
        return Ok(await _orderService.GetTotalOrder()); 
    }
}