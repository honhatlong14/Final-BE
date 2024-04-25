using System.Text.Json.Serialization;
using Common.Constants;

namespace Data.ViewModels.User;

public class AuthenticateResponse
{
    public AuthenticateResponse(Entities.User.User user, string jwtToken, string refreshToken)
    {
        Id = user.Id;
        Email = user.Email;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
        Role = user.Role;
        IsVerified = user.IsVerified;
        AccountStatus = user.AccountStatus; 
    }
    
    public AuthenticateResponse()
    {
    }
    public string Id { get; set; }
    public string JwtToken { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public StringEnum.AccountStatus AccountStatus { set; get; }
    public string Email { get; set; }
    public StringEnum.Roles Role { get; set; }
    public bool IsVerified { get; set; }

   
        
   
}