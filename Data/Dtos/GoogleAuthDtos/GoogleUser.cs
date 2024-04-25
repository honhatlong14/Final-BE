namespace Data.Dtos.GoogleAuthDtos;

public class GoogleUser
{
    public string Sub { get; set; }
    public string Email { get; set; }
    public string Family_Name { get; set; }
    public string Given_Name { get; set; }
    public bool Email_Verified { get; set; }
    public string Picture { get; set; }
    public string PhoneNumber { get; set; }
}