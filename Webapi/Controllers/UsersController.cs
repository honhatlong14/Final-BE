using AutoMapper;
using Common.Constants;
using Data.Dtos.GoogleAuthDtos;
using Data.Entities.User;
using Data.ViewModels;
using Data.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Webapi.Attributes;

namespace Webapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService; 

    public UsersController(IUserService userService, IMapper mapper, IImageService imageService)
    {
        _userService = userService;
        _mapper = mapper;
        _imageService = imageService; 
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
    {
        var response = await _userService.Authenticate(model, IpAddress());
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("authenticate-google")]
    public async Task<ActionResult<AuthenticateResponse>> AuthenticateGoogleLogin(GoogleLoginRequest model)
    {
        var response = await _userService.AuthenticateGoogleLogin(model, IpAddress(), Request.Headers["origin"]);
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthenticateResponse>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userService.RefreshToken(refreshToken, IpAddress());
        if (response == null)
            return BadRequest();
        SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
    {
        // accept token from request body or cookie
        var token = model.RefreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        // users can revoke their own tokens and admins can revoke any tokens
        // if (!Account.OwnsToken(token) && Account.Role != StringEnum.Roles.Admin)
        //     return Unauthorized(new { message = "Unauthorized" });

        await _userService.RevokeToken(token, IpAddress());
        return Ok(new { message = "Token revoked" });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm]RegisterRequest model)
    {
        var user = _mapper.Map<User>(model);
        user.Avatar = await _imageService.UploadImage(model.AvatarFile); 
        user.Role = StringEnum.Roles.User;
        await _userService.Register(user, model.Password, Request.Headers["origin"], string.Empty);
        return Ok(new { message = "Registration successful, please check your email for verification instructions" });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
    {
        await _userService.VerifyEmail(model.Token);
        return Ok(new { message = "Verification successful, you can now login" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await _userService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok(new { message = "Please check your email for password reset instructions" });
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
    {
        await _userService.ValidateResetToken(model);
        return Ok(new { message = "Token is valid" });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        await _userService.ResetPassword(model);
        return Ok(new { message = "Password reset successful, you can now login" });
    }

    [Authorize(StringEnum.Roles.Admin)]
    [HttpGet("getAllActivateAccount")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAllActivateAccount()
    {
        var accounts = await _userService.GetAllActivateAccount();
        var response = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
        return Ok(response);
    }
    
    [Authorize(StringEnum.Roles.Admin)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll()
    {
        var accounts = await _userService.GetAll();
        var response = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponse>> GetById(string id)
    {
        // users can get their own account and admins can get any account
        if (id != Account.Id && Account.Role != StringEnum.Roles.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        var account = await _userService.GetById(id);
        return Ok(account);
    }

    [Authorize(StringEnum.Roles.Admin, StringEnum.Roles.Staff)]
    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create([FromForm]CreateRequest model)
    {
        var user = _mapper.Map<User>(model);
        user.Avatar = await _imageService.UploadImage(model.AvatarFile); 
        if (model.Role == StringEnum.Roles.Admin && Account.Role != StringEnum.Roles.Admin)
            return BadRequest(new { message = "You need to be admin to register" });

        await _userService.Create(user, model.Password);
        return Ok(new { message = "Create successful" });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<AccountResponse>> Update(string id,[FromForm] UpdateRequest model)
    {
        // users can update their own account and admins can update any account
        if (id != Account.Id && Account.Role != StringEnum.Roles.Admin)
            return Unauthorized(new { message = "Unauthorized" });
        
        var avatarUser = await _imageService.UploadImage(model.AvatarFile);
        
        var user = _mapper.Map<User>(model);
        user.Avatar = avatarUser; 
        var account = await _userService.Update(id, user, UserId, model.Password);
        return Ok(account);
    }
    
    [AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        // users can delete their own account and admins can delete any account
        // if (id != Account.Id && Account.Role != StringEnum.Roles.SeniorAdmin)
        //     return Unauthorized(new { message = "Unauthorized" });

        await _userService.Delete(id);
        return Ok(new { message = "Account deleted successfully" });
    }

    [HttpPut("activate/{id}")]
    public async Task<IActionResult> Activate(string id)
    {
        // users can delete their own account and admins can delete any account
        // if (id != Account.Id && Account.Role != StringEnum.Roles.SeniorAdmin)
        //     return Unauthorized(new { message = "Unauthorized" });

        await _userService.Activate(id);
        return Ok(new { message = "Account Re Activate successfully" });
    }

    // helper methods
    
    // set cookie để làm gì 
    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private string IpAddress()
    {
        // tại sao lại Constains Key như thế 
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }

    [HttpPut("changePassword/{id}")]
    public async Task<IActionResult> UpdatePassword(string id, [FromBody]UpdatePasswordRequest model)
    {
        await _userService.ChangePassword(id, model);
        return Ok("Change Password Success");
    }
    
    [HttpPut("changePhoneNumber/{id}")]
    public async Task<IActionResult> UpdatePhoneNumber(string id, [FromBody]UpdatePhoneNumberRequest model)
    {
        await _userService.ChangePhoneNumber(id, model);
        return Ok("Change Phone Number Success");
    }
    
    [HttpPut("updateImage/{id}")]
    public async Task<IActionResult> UpdateImage(string id, [FromForm]UpdateImageRequest model)
    {
        var image = await _imageService.UploadImage(model.AvatarFile);
        await _userService.UpdateImage(id,image);
        return Ok("Update Image Success");
    }

    [AllowAnonymous]
    [HttpGet("getProfitByUserRole")]
    public async Task<ActionResult<string>> GetProfitByUserRole()
    {
        var profit = await _userService.GetProfitByUserRole();
        return Ok(profit); 
    }
    
    [AllowAnonymous]
    [HttpGet("getProfitCountingUser")]
    public async Task<ActionResult<string>> GetProfitCountingUser()
    {
        var profit = await _userService.GetProfitCountingUser();
        return Ok(profit); 
    }
}