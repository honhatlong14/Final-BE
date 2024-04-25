using System.Text.Json.Serialization;

namespace Backend.Dtos.ChatDtos;

public class ChatModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<UserInChatSignalR> Users { get; set; } // Represents the users involved in the chat
    public List<object> Messages { get; set; } // Represents the chat message content
}
    
public class UserInChatSignalR
{
    public string Id { get; set; }
    public string Avatar { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Gender = "male";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [JsonIgnore]
    public string Status { get; set; }
}

public class AddFriendModel
{
    public List<ChatModel> Chats { get; set; }
}