using Data.Entities;

namespace Backend.Dtos.ChatDtos;

public class CreateChatResponse
{
    public List<CreateChatResponseModel> CreateChatResponseModels { get; set; }
}

public class CreateChatResponseModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Object ChatUser { get; set; }
    public List<UserInMessage> Users { get; set; }
    public List<ChatMessage> Messages = new List<ChatMessage>();
}
