namespace Backend.Dtos.ChatDtos;

public class DeleteChatModel
{
    public string ChatId { get; set; }
    public List<string> NotifyUsers { get; set; }
}