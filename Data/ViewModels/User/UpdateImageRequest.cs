using Microsoft.AspNetCore.Http;

namespace Data.ViewModels.User;

public class UpdateImageRequest
{
    public IFormFile AvatarFile { get; set; }
}