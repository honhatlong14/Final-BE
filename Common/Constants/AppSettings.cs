namespace Common.Constants;

public class AppSettings
{
    public static string Secret { get; set; }
    public static int RefreshTokenTTL { get; set; }
    public static string[] CORS { get; set; }
    public static string GoogleClientId { get; set; }
        
    public static string GoogleClientSecret { get; set; }
}