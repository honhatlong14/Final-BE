using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Webapi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : BaseApiController
{
    private readonly ICategoryService _categoryService; 
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService; 
    }
    
    [HttpGet("getAllActivate")]
    public async Task<IEnumerable<Category>> GetAllActivate()  
    {
        var categories = await _categoryService.GetAllActivate();
        return categories; 
    }
    
    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromQuery]string categoryName)  
    {
        var category = await _categoryService.Create(categoryName);
        return category; 
    }
    
    [HttpGet]
    public async Task<IEnumerable<Category>> GetAll()  
    {
        var categories = await _categoryService.GetAll();
        return categories; 
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Category>> Delete(string id)
    {
        await _categoryService.Delete(id);
        return Ok("Delete successfully"); 
    }
}