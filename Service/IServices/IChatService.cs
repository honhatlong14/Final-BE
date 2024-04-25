using Backend.Dtos.ChatDtos;

namespace Service.IServices;


public interface IChatService
{
    Task<List<GetChatByUserIdResponse>> GetChatByUserId(string id);
    Task<CreateChatResponse> Create(string partnerId, string userId);
    Task<object> Messages(string id, int page = 1);
    Task<object> Delete(string chatId);
    object UpLoadImage(Stream image);
}