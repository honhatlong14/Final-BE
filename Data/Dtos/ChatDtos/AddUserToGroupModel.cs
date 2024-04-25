namespace Backend.Dtos.ChatDtos;

public class AddUserToGroupModel
{
    public ChatModel Chat { get; set; }
    public UserInChatSignalR NewChatter { get; set; }
}