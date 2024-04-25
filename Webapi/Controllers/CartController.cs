using AutoMapper;
using Data.Entities;
using Data.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : BaseApiController
{
    private readonly ICartService _cartService;
    private readonly IPostService _postService;

    private readonly IMapper _mapper;
    private readonly IImageService _image;

    public CartController(ICartService cartService, IPostService postService, IMapper mapper, IImageService image)
    {
        _cartService = cartService;
        _mapper = mapper;
        _image = image;
        _postService = postService;
    }

    [HttpGet]
    public async Task<IEnumerable<Cart>> GetAll()
    {
        return await _cartService.GetAll();
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Cart>> Update(string id, UpdateCartRequest model)
    {
        var cart = _mapper.Map<Cart>(model);
        var cartUpdate = await _cartService.Update(id, UserId, cart);
        return Ok(cartUpdate);
    }

    [HttpGet("{userId}")]
    public async Task<IEnumerable<GetAllCartResponse>> GetAllByUserId(string userId)
    {
        var response = new List<GetAllCartResponse>();
        var carts = await _cartService.GetAllByUserId(userId);

        foreach (var item in carts)
        {
            var post = await _postService.GetByBookId(item.BookId);
            var data = new GetAllCartResponse()
            {
                Id = item.Id,
                StallName = post.Stall.StallName,
                StallId = post.StallId, 
                Book = item.Book,
                Quantity = item.Quantity,
                UserId = item.UserId,
                IsSelected = item.IsSelected, 
                Total = item.Total
                
            };
            response.Add(data);
        }

        return response;
    }  
    
    [HttpGet("itemCartSelected/{userId}")]
    public async Task<ActionResult<IEnumerable<Cart>>> GetAllCartSelected(string userId)
    {
        return Ok(await _cartService.GetAllCartSelected(userId));
    }

    [HttpPost("reduceQuantity")]
    public async Task<ActionResult<Cart>> ReduceQuantity([FromBody] ReduceQuantityRequest model)
    {
        return await _cartService.ReduceQuantity(model.CartId, model.UserId); 
    }
    
    [HttpPost("incrementQuantity")]
    public async Task<ActionResult<Cart>> IncrementQuantity([FromBody] IncrementQuantityRequest model)
    {
        return await _cartService.IncrementQuantity(model.CartId, model.UserId); 
    }
    
    [HttpPost("addToCart")]
    public async Task<ActionResult<Cart>> AddToCart([FromBody] CreateCardRequest model)
    {
        // return await _cartService.AddToCart(model.BookId, model.UserId, model.Quantity, model.Total);
        return await _cartService.AddToCart(model.BookId, UserId, model.Quantity, model.Total);

    }

    [HttpPost("removeItemByUserId")]
    public async Task<ActionResult<Cart>> RemoveByUserId([FromBody] RemoveCartItemRequest model)
    { 
        return await _cartService.RemoveByUserId(model.CartId, model.UserId);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Cart>> Delete(string id)
    {
        await _cartService.Delete(id);
        return Ok("Delete cart successfully"); 
    }
    
}