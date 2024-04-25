namespace Service.IServices;

public interface IJwtUtils
{
    public string? ValidateToken(string token);
}