namespace Data.ViewModels.User;

public class UpdatePasswordRequest
{
    public string Password { set; get; }
    public string ConfirmPassword { set; get; }
}