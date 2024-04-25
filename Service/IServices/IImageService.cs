using CloudinaryDotNet.Actions;
using Data.Entities;
using Data.Entities.User;
using Data.ViewModels.Book;
using Microsoft.AspNetCore.Http;

namespace Service.IServices;

public interface IImageService
{
    Task<string> UploadImage(IFormFile file);
    Task<Book> UploadImages(IList<IFormFile> files, Book model);
    Task<IList<ImageUploadResult>> UploadImageBase64(IList<string> listBase64File);
    Task<Book> UploadImageBase64(IList<string> listBase64File, Book book);
    Task<Book> UploadImageBase64(IList<ImageDataUpload> listImageUpload, Book book, string bookId);
    ImageUploadResult? UploadFile(Stream fileStream, string fileName);
    bool DeleteFile(string publicId);
}