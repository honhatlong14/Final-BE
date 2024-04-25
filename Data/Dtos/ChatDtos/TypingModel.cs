namespace Backend.Dtos.ChatDtos;

public class TypingModel
{
    public string ChatId { get; set; }
    public UserInMessage FromUser { get; set; }
    public List<string> ToUserId { get; set;  }
    public bool Typing { get; set;  }
}