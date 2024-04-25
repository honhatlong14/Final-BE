using Data.Dtos.GoogleAuthDtos;
using Data.Entities.User;
using Data.ViewModels.User;

namespace Service.IServices;

public interface IUserService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
    Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
    Task<bool> RevokeToken(string token, string ipAddress);
    Task Register(User model, string password, string origin, string userId);
    Task VerifyEmail(string token);
    Task ForgotPassword(ForgotPasswordRequest model, string origin);
    Task ValidateResetToken(ValidateResetTokenRequest model);
    Task ResetPassword(ResetPasswordRequest model);
    Task<IEnumerable<User>> GetAll();  
    Task<IEnumerable<User>> GetAllActivateAccount();  
    Task<User> GetById(string id);
    Task<User> Create(User account, string password);
    Task<User> Update(string id, User model, string userId, string password);
    Task Delete(string id);
    Task Activate(string id);
    Task SwitchActivateAccount(string id);
    Task<string> ChangePassword(string id, UpdatePasswordRequest model);
    Task<string> ChangePhoneNumber(string id, UpdatePhoneNumberRequest model);
    Task<User> UpdateImage(string id, string avatar);

    Task<string> GetProfitByUserRole(); 
    Task<string> GetProfitCountingUser(); 
    Task<AuthenticateResponse> AuthenticateGoogleLogin(GoogleLoginRequest model, string ipAddress, string origin);

}