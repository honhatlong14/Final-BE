using Service.IServices;

namespace Webapi.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateToken(token);
        if (userId != null)
        {
            var user = await userService.GetById(userId);
            // attach user to context on successful jwt validation
            context.Items["User"] = user;
            context.Items["UserId"] = userId;
            context.Items["Role"] = user.Role;
        }
            
        await _next(context);
    }
}