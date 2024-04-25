namespace Backend.Dtos.ChatDtos;

public class MessageSocket
{
    public string Type { get; set; }
    public UserInMessage FromUser  { get; set; }
    public List<string> ToUserId  { get; set; }
    public string ChatId  { get; set; }
    public string Message  { get; set; }
}