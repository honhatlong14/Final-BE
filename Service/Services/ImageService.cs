using System.Collections;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Entities;
using Data.Entities.User;
using Data.IRepository;
using Data.ViewModels.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;


namespace Service.Services;

public class ImageService : IImageService
{
    
    private readonly IRepository<Image> _imageRepo; 
    private Cloudinary _cloudinary;

    public ImageService(IRepository<Image> imageRepo)
    {
        Account account = new Account(
            "duvhinrd0",
            "643781911967294",
            "kqhZHT7pLmLWayEfVAM53chbkQI"
        );

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
        _imageRepo = imageRepo; 
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            PublicId = Guid.NewGuid().ToString()
        };

        var uploadResult = _cloudinary.Upload(uploadParams);
        return uploadResult.SecureUri.ToString();
    }
    
    

    public async Task<IList<ImageUploadResult>> UploadImageBase64(IList<string> listBase64Files)
    {
        IList<ImageUploadResult> results = new List<ImageUploadResult>();
        foreach (var item in listBase64Files)
        {
            // string convert = item.Replace("data:image/jpeg;base64,", String.Empty);
            string validBase64String = RemoveImagePrefix(item);
            var bytes = Convert.FromBase64String(validBase64String);
            var contents = new StreamContent(new MemoryStream(bytes)).ReadAsStream();
        
            // var streamFile = new MemoryStream(Convert.FromBase64String(base64File));
        
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), contents),
                PublicId = Guid.NewGuid().ToString()
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            results.Add(uploadResult);
        }
       
        return results;
    }

    public async Task<Book> UploadImageBase64(IList<string> listBase64File, Book book)
    {
        var listImages = new List<Image>();
        var uploadResult = await UploadImageBase64(listBase64File); 
        foreach (var item in uploadResult)
        {
            var image = new Image
            {
                BookId = book.Id,
                ImageUrl = item.Url.ToString()
            }; 
            listImages.Add(image);  
        }

        book.Images = listImages;
        return book; 
    }
    
    
    public async Task<Book> UploadImageBase64(IList<ImageDataUpload> listImageUpload, Book book, string bookId)
    {
        var listImages = new List<Image>();

        // Lấy danh sách Id của Images trong cơ sở dữ liệu
        var existingImageIds =  (await _imageRepo.GetAllAsync(x => x.BookId == bookId)).Select(i => i.Id);

        // Tạo danh sách các Id của ảnh trong listImageUpload
        var newImageIds = listImageUpload.Where(img => !img.Id.IsNullOrEmpty()).Select(img => img.Id).ToList();

        // Duyệt qua danh sách Id của Images trong cơ sở dữ liệu
        foreach (var existingImageId in existingImageIds)
        {
            // Nếu Id của ảnh trong cơ sở dữ liệu không có trong danh sách mới, xóa nó
            if (newImageIds.Contains(existingImageId))
            {
                // Loại bỏ ảnh từ danh sách hiện tại
                var existingImage = (await _imageRepo.GetFirstOrDefaultAsync(img => img.Id == existingImageId));
                // book.Images.Remove(existingImage);
                listImages.Add(existingImage);
                //
                // // Xóa ảnh từ cơ sở dữ liệu (đây là nơi cần triển khai phương thức xóa từ cơ sở dữ liệu)
                // _yourImageRepository.DeleteImage(existingImageId);
            }
        }

        // Duyệt qua danh sách ảnh mới
        foreach (var imageDataUpload in listImageUpload)
        {
            // Nếu Id của ImageDataUpload là null, thì xử lý ảnh và thêm vào book
            if (imageDataUpload.Id.IsNullOrEmpty())
            {
                var uploadResult = await UploadImageBase64(new List<string> { imageDataUpload.DataImage });

                foreach (var item in uploadResult)
                {
                    var image = new Image
                    {
                        BookId = book.Id,
                        ImageUrl = item.Url.ToString()
                    };
                    listImages.Add(image);
                }
            }
        }

        // Gán danh sách ảnh mới vào book
        book.Images = listImages;

        return book;
    }


    static string RemoveImagePrefix(string base64String)
    {
        // Tách phần sau dấu phẩy để lấy dữ liệu Base-64
        int commaIndex = base64String.IndexOf(",");
        if (commaIndex >= 0 && commaIndex < base64String.Length - 1)
        {
            return base64String.Substring(commaIndex + 1);
        }
        return base64String;
    }

    public async Task<Book> UploadImages(IList<IFormFile> files, Book model)
    {
        List<string> imageUrls = new List<string>();
        List<Image> images = new List<Image>(); 
        foreach (var item in files)
        {
            var uploadResult = await UploadImage(item); 
            imageUrls.Add(uploadResult);
        }

        foreach (var UrlItem in imageUrls)
        {
            var image = new Image
            {
                BookId = model.Id, 
                ImageUrl = UrlItem
            };
            images.Add(image); 
        }

        model.Images = images; 
        
        return model;
    }
    
    public ImageUploadResult? UploadFile(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams(){
            File = new FileDescription(fileName, fileStream),
        };
        
        var uploadResult = _cloudinary.Upload(uploadParams);
        
        return uploadResult;
    }

    public bool DeleteFile(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var deletionResult = _cloudinary.Destroy(deletionParams);
        
        if (deletionResult.Result == "ok")
        {
            return true;
        }

        return false;
    }
}