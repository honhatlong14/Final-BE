namespace Backend.Dtos.ChatDtos;

public class GetChatByUserIdResponse
{
    public string Id { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChatUserResponse ChatUser { get; set; }
    // list of user info we chat with
    public List<UserResponse> Users { get; set; }
    public List<MessageResponse> Messages { get; set; }
}

public class ChatUserResponse
{
    public string ChatId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
    
public class UserResponse
{
    public string Id { get; set; }
    public string Avatar { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Gender = "male";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChatUserResponse ChatUser { get; set; }
}

public class MessageResponse
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public string ChatId { get; set; }
    // TODO: ???? 
    public string FromUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // info of user who write this message
    public UserInMessage User { get; set; }
}
    
public class UserInMessage
{
    public string Id { get; set; }
    public string Avatar { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Gender = "male";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}