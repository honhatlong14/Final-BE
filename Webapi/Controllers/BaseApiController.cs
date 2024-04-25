using Data.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Webapi.Configurations;

namespace Webapi.Controllers;

public abstract class BaseApiController : ControllerBase
{ 
    public string UserCurrentRole => HttpContext.GetRole();

    /// <summary>
    /// The current user logged in.
    /// </summary>
    protected string UserId => HttpContext.GetUserId();

    public User Account => (User)HttpContext.Items["User"];

    protected (IList<IFormFile> attachmentFiles, IList<IFormFile> imageInLineFiles) Attachment()
    {
        var imageInLineFiles = new List<IFormFile>();
        var attachmentFiles = new List<IFormFile>();
        var request = HttpContext.Request;
        foreach (var file in request.Form.Files)
        {
            if (file.Name.Contains("imageInLine"))
            {
                imageInLineFiles.Add(file);
            }
            else
            {
                attachmentFiles.Add(file);
            }
        }

        return (attachmentFiles, imageInLineFiles);
    }   
}