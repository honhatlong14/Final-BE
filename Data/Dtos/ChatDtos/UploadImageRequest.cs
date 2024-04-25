using Microsoft.AspNetCore.Http;

namespace Backend.Dtos.ChatDtos;

public class UploadImageRequest
{
    public IFormFile image { get; set; }
    public int id { get; set; }
}