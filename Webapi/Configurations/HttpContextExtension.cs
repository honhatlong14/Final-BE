namespace Webapi.Configurations;

public static class HttpContextExtension
{
    private static string GetContextValue(HttpContext context, string claimName)
    {
        return context.Items.FirstOrDefault(x => (string)x.Key == claimName).Value.ToString();
    }
        
    public static string GetUserId(this HttpContext context)
    {
        return GetContextValue(context, "UserId");
    }

    /// <summary>
    /// There are 2 roles: Admin and User
    /// </summary>
    public static string GetRole(this HttpContext context)
    {
        return GetContextValue(context, "Role");
    }
}