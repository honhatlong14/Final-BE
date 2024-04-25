namespace Backend.Dtos.ChatDtos;

public class UserConnection
{
    public string Id { get; set; }
    public HashSet<string> Sockets { get; set; }
}