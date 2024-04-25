using AutoMapper;
using Data.Entities;
using Data.ViewModels.Address;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;



[ApiController]
[Route("api/[controller]")]
public class AddressController : BaseApiController
{
    private readonly IAddressService _addressService;
    private readonly IMapper _mapper;
    private readonly IImageService _image; 
    
    public AddressController(IAddressService addressService, IMapper mapper, IImageService image)
    {
        _addressService = addressService;
        _mapper = mapper;
        _image = image; 
    }
    
    [HttpGet]
    public async Task<IEnumerable<Address>> GetAll()
    {
        var addresses = await _addressService.GetAll();
        return addresses; 
    }
    
    [HttpPost]
    public async  Task<ActionResult<Address>> Create([FromBody] CreateAddressRequest model)
    {
        var address = _mapper.Map<Address>(model);
        return await _addressService.Create(UserId, address);; 
    }
    
    [HttpGet("getByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<Address>>> GetByUserId(string userId)
    {
        var addresses = await _addressService.GetByUserId(userId);
        return Ok(addresses);
    }
    
    [HttpGet("getDefaultByUserId/{userId}")]
    public async Task<ActionResult<Address>> GetDefaultByUserId(string userId)
    {
        var addresses = await _addressService.GetDefaultAddressByUserId(userId);
        return Ok(addresses);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Address>> Delete(string id)
    {
        await _addressService.Delete(id);
        return Ok("Delete successfully"); 
    }
}

