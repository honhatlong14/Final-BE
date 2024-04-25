namespace Backend.Dtos.ChatDtos;

public class AddUserToGroupRequest
{
    public string ChatId { get; set; }
    public string UserId { get; set; }
}