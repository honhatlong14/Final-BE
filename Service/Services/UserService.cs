
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Common.Constants;
using Common.Exception;
using Data.DbContext;
using Data.Dtos.GoogleAuthDtos;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.IRepository;
using Data.Repository;
using Data.ViewModels.User;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Service.IServices;
using Service.Utility;  

namespace Service.Services;

public class UserService : IUserService
{
    // repository area
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Stall> _stallRepo;
    private readonly ApplicationDbContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    // service area
    private readonly ISendMailService _emailService;

    public UserService(
        IRepository<User> userRepo,
        ISendMailService emailService,
        ApplicationDbContext context, 
        IJwtUtils jwtUtils,
        IMapper mapper
       )
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _context = context; 
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var user = await _userRepo.GetFirstOrDefaultAsync(x =>
            x.Email.ToLower() == model.Email.ToLower() && !x.IsDeleted);
        
        if (user == null)
            throw new Exception("User not have register or has been locked");
        
        if (user.Verified == null)
            throw new Exception("Email need to be verify");
        
        if (user.AccountStatus == StringEnum.AccountStatus.Lock)
            throw new Exception("User Lock");
        
        var passwordHash = Encryption.DecryptPassword(user, model.Password);
        // validate
        
        if (user == null || passwordHash != user.PasswordHash)
            throw new Exception("Username or password is incorrect");

       

        // authentication successful so generate jwt and refresh tokens
        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(ipAddress);

        // save refresh token
        if (user.RefreshTokens == null) 
            user.RefreshTokens = new List<RefreshToken>();

        user.RefreshTokens.Add(refreshToken);

        // remove old refresh tokens from account
        RemoveOldRefreshTokens(user);   
        
        // update db
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        // update refreshToken
        await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, ipAddress);

        return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
    }

    public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
    {
        var user = await _userRepo.GetFirstOrDefaultAsync(u =>
            u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted);

        // return null if no user found with token
        if (user == null) return null;

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        // return null if token is no longer active
        if (!refreshToken.IsActive) return null;

        // replace old refresh token with a new one and save
        var newRefreshToken = GenerateRefreshToken(ipAddress);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        user.RefreshTokens.Add(newRefreshToken);

        RemoveOldRefreshTokens(user);

        // update db
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        // update refreshToken 
        await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, ipAddress);

        // generate new jwt
        var jwtToken = GenerateJwtToken(user);


        return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
    }

    public async Task<bool> RevokeToken(string token, string ipAddress)
    {
        var user = await _userRepo.GetFirstOrDefaultAsync(u =>
            u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted);

        // return false if no user found with token
        if (user == null) return false;

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        // return false if token is not active
        if (!refreshToken.IsActive) return false;

        // revoke token and save
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        
        // update db
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        // update refreshToken 
        await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, ipAddress);

        return true;
    }

    public async Task<IEnumerable<User>> GetAllActivateAccount()
    {
        return await _userRepo.GetAllAsync(_ => !_.IsDeleted);
    }
    
    public async Task<IEnumerable<User>> GetAll()
    {
        return await _userRepo.GetAllAsync();
    }

    public async Task<User> GetById(string id)
    {
        var result = await GetAccount(id);
        return result;
    }

    // change BaseUser -> User and replace userRepository with new User repo
    public async Task Register(User model, string password, string origin, string userId)
    {
        try
        {
            // validate
            var userList =
                await _userRepo.GetFirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower() && !x.IsDeleted);
            if (userList != null)
            {
                await SendAlreadyRegisteredEmail(model.Email.ToLower(), origin);
                throw new AppException("" + model.Email.ToLower() + "' is already taken");
            }

            if (model.Email == null)
                throw new AppException("Email is not empty!");
            if (model.FullName == null)
                throw new AppException("Full name is not empty!");
            if (model == null)
                throw new AggregateException("Account not found"); 
 
            // hash password
            Encryption.EncryptPassword(model, password);

            model.AccountStatus = StringEnum.AccountStatus.Active; 
            model.VerificationToken = RandomTokenString();
            model.CreatedBy = userId;
            model.CreateAt = DateTime.Now;
            model.AccountStatus = StringEnum.AccountStatus.Active;
            model.IsInternal = true; 
            

            // save user
            await _userRepo.AddAsync(model);
            await _userRepo.SaveChangesAsync();

            // send email
            await SendVerificationEmail(model, origin);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    // helper methods

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Role", user.Role.ToValue())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }

    public async Task VerifyEmail(string token)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x => x.VerificationToken == token);

        if (account == null) throw new AppException("Verification failed");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();
    }

    public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower());

        // always return ok response to prevent email enumeration
        if (account == null) return;

        // create reset token that expires after 1 day
        account.ResetToken = RandomTokenString();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();

        // send email
        SendPasswordResetEmail(account, origin);
    }

    public async Task ValidateResetToken(ValidateResetTokenRequest model)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x =>
            x.ResetToken == model.Token &&
            x.ResetTokenExpires > DateTime.UtcNow);

        if (account == null)
            throw new AppException("Invalid token");
    }

    public async Task ResetPassword(ResetPasswordRequest model)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x =>
            x.ResetToken == model.Token &&
            x.ResetTokenExpires > DateTime.UtcNow);

        if (account == null)
            throw new AppException("Invalid token");

        // hash password
        Encryption.EncryptPassword(account, model.Password);
        // update password and remove reset token
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;
        account.UpdateAt = DateTime.UtcNow;

        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();
    }

    public async Task<User> Create(User account, string password)
    {
        // validate
        if ((await _userRepo.GetFirstOrDefaultAsync(x => x.Email == account.Email)) != null)
            throw new AppException($"Email '{account.Email}' is already registered");

        // map model to new account object
        account.CreateAt = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;

        // hash password
        Encryption.EncryptPassword(account, password);
        // save user
        await _userRepo.AddAsync(account);
        await _userRepo.SaveChangesAsync();

        return account;
    }

    public async Task<User> Update(string id, User model, string userId, string password)
    {
        var account = await GetAccount(id);

        // validate
        if (account.Email != model.Email &&
            (await _userRepo.GetFirstOrDefaultAsync(x => x.Email == model.Email)) != null)
            throw new AppException($"Email '{model.Email}' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(password))
            Encryption.EncryptPassword(account, password);
        
        // copy model to account and save
        account.UpdateAt = DateTime.UtcNow;
        account.UpdateBy = userId;
        account.FullName = model.FullName;
        account.Avatar = model.Avatar; 
        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();

        return account;
    }

    public async Task Delete(string id)
    {
        var account = await GetAccount(id);
        account.IsDeleted = true;
        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();
    }

    public async Task Activate(string id)
    {
        var account = await GetDeletedAccount(id);
        account.IsDeleted = false;
        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();
    }

    public async Task SwitchActivateAccount(string id)
    {
        var user = await GetAccount(id);
        if (user.IsDeleted == false)
        { 
            await Delete(id); 
        }
        if (user.IsDeleted == true)
        {
            await Activate(id); 
        }
    }

    // helper methods

    private async Task<User> GetAccount(string id)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }

    private async Task<User> GetDeletedAccount(string id)
    {
        var account = await _userRepo.GetFirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }

    private void RemoveOldRefreshTokens(User account)
    {
        account.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.Created?.AddDays(AppSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private string RandomTokenString()
    {
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[40];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        // convert random bytes to hex string
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    private async Task SendVerificationEmail(User account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var verifyUrl = $"{origin}/verify-email?token={account.VerificationToken}";
            //message = await File.ReadAllTextAsync("../Service/HtmlTemplates/VerifyEmail.html");
            // message = "[[VerifyLink]]";
            // message = message.Replace("[[VerifyLink]]", verifyUrl);
            message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            message = $@"
                        <h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        <p>Please use the below token to verify your email address with the <code>/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
        }

        await _emailService.SendEmailAsync(
            account.Email,
            subject: "Sign-up Verification - Verify Email",
            message
        );
    }

    private async Task SendAlreadyRegisteredEmail(string email, string origin)
    {
        string message;
        // if (!string.IsNullOrEmpty(origin))
        // {
        //     //message = await File.ReadAllTextAsync("../Service/HtmlTemplates/AccountExisted.html");
        //     message = "[[ResetPassword]]";
        //     message = message.Replace("[[ResetPassword]]", $"{origin}/login");
        // }
        // else
        //     message = $@"<h4>Email Already Registered</h4>
        //                  <p>Your email <strong>{email}</strong> is already registered.</p>
        //                  <p>If you don't know your password you can reset it via the <code>/forgot-password</code> api route.</p>";
        //
        // await _emailService.SendEmailAsync(
        //     email,
        //     "Sign-up Verification API - Email Already Registered",
        //     message
        // );
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
        else
            message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

        await _emailService.SendEmailAsync(
            email,
            "Sign-up Verification API - Email Already Registered",
            $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
        );
    }

    private async Task SendPasswordResetEmail(User account, string origin)
    {
        // string message;
        // if (!string.IsNullOrEmpty(origin))
        // {
        //     var resetUrl = $"{origin}/reset-password?token={account.ResetToken}";
        //     //message = await File.ReadAllTextAsync("../Service/HtmlTemplates/ResetPassword.html");
        //     message = "[[ResetLink]]";
        //     message = message.Replace("[[ResetLink]]", resetUrl);
        // }
        // else
        // {
        //     message = $@"<h4>Reset Password Email</h4>
        //                     <p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
        //                     <p>Please use the below token to reset your password with the <code>/reset-password</code> api route:</p>
        //                      <p><code>{account.ResetToken}</code></p>";
        // }
        //
        // await _emailService.SendEmailAsync(
        //     account.Email,
        //     subject: "Sign-up Verification API - Reset Password",
        //     message
        // );
        
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
            message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
        }

        await _emailService.SendEmailAsync(
            account.Email,
            subject: "Sign-up Verification API - Reset Password",
            $@"<h4>Reset Password Email</h4>
                         {message}"
        );
    }
    
    
    public async Task<string> ChangePassword(string id, UpdatePasswordRequest model)
    {
        var user = await GetAccount(id); 
        if (user == null) throw new AppException("User or ID user can not found");
        if (model.Password != model.ConfirmPassword) throw new AppException("Password and Confirm Password not match");

        user.PasswordHash = Encryption.DecryptPassword(user, model.Password);
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        return user.PasswordHash; 
    }

    public async Task<string> ChangePhoneNumber(string id, UpdatePhoneNumberRequest model)
    {
        var user = await GetAccount(id); 
        if (user == null) throw new AppException("User or ID user can not found");

        user.PhoneNumber = model.PhoneNumber; 
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        return user.PhoneNumber; 
    }

    public async Task<User> UpdateImage(string id, string avatar)
    {
        var account = await GetAccount(id); 
        if (account == null) throw new AppException("User or ID user can not found");
        if(avatar == null) throw new AppException("Avatar is null or not selected");

        account.Avatar = avatar;
        _userRepo.Update(account);
        await _userRepo.SaveChangesAsync();
        return account; 
    }

    public async Task<string> GetProfitByUserRole()
    {
        var profit = (await _userRepo.GetAllAsync()).GroupBy(g => g.Role).Select(g => new
        {
            role = g.Key,
            userCount = g.Count(), 
        });

        var jsonData = JsonConvert.SerializeObject(profit, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );

        return jsonData;
    }

    public async Task<string> GetProfitCountingUser()
    {
        var usersCounting = (await _userRepo.GetAllAsync()).Count();
        var jsonData = JsonConvert.SerializeObject(usersCounting, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );

        return jsonData;
    }

    public async Task<AuthenticateResponse> AuthenticateGoogleLogin(GoogleLoginRequest model, string ipAddress, string origin)
        {
            var googleUser = ValidateGoogleToken(model.TokenId);
            
            var existingUser = await _userRepo.GetFirstOrDefaultAsync(u => u.Email == googleUser.Email);
            
            if (existingUser == null)
            {
                // User doesn't exist, create a new user
                existingUser = CreateUserFromGoogleData(googleUser);

                // Save the new user to the database
                await _userRepo.AddAsync(existingUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!existingUser.IsInternal)
                {
                    // User exists, update any necessary information
                    existingUser = UpdateUserFromGoogleData(existingUser, googleUser);
                    // Save the updated user to the database
                     _userRepo.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    SendAlreadyRegisteredEmail(existingUser.Email, origin);
                    return new AuthenticateResponse();
                }
            }

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(existingUser);
            var refreshToken = GenerateRefreshToken(ipAddress);
            existingUser.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(existingUser);

            // save changes to db
            _context.Update(existingUser);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(existingUser);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }
    
    private GoogleUser ValidateGoogleToken(string googleToken)
    {
        using (HttpClient client = new HttpClient())
        {
            // Set up the request to Google's token validation endpoint
            var requestUri = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={googleToken}&client_id={AppSettings.GoogleClientId}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            // Send the request and get the response
            var response = client.SendAsync(request).Result;

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Parse the response content
                var responseContent = response.Content.ReadAsStringAsync().Result;
            
                // Deserialize the JSON response to retrieve user information
                var googleUser = JsonConvert.DeserializeObject<GoogleUser>(responseContent);

                return googleUser;
            }
            else
            {
                throw new AppException($"Google token validation failed. Status code: {response.StatusCode}");
            }
        }
    }
    
    private User CreateUserFromGoogleData(GoogleUser googleUser)
    {
        var newUser = new User
        {
            FullName= googleUser.Given_Name + " " + googleUser.Family_Name,
            Email = googleUser.Email,
            Role = StringEnum.Roles.User,
            AcceptTerms = true,
            Verified = DateTime.UtcNow,
            CreateAt = DateTime.UtcNow,
            Avatar = googleUser.Picture,
            IsInternal = false,
            RefreshTokens = new List<RefreshToken>(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("GoogleStrongPass"),
            AccountStatus = StringEnum.AccountStatus.Active
        };
        return newUser;
    }
        
    private User UpdateUserFromGoogleData(User existingUser, GoogleUser googleUser)
    {
        existingUser.Email = googleUser.Email;
        existingUser.FullName = googleUser.Given_Name + " " + googleUser.Family_Name;
        existingUser.Avatar = googleUser.Picture;
        return existingUser;
    }
    
}