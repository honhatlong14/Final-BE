    using Common.Constants;

    namespace Data.ViewModels;

public class AccountResponse
{
    public string Id { get; set; }
    public string Password { get; set; }
    public string Field { get; set; }
    public DateTime? CreateAt { get; set; }
    public string SchoolYear { get; set; }
    public string Role { get; set; }
    public string IdStudent { get; set; } 
    public string CitizenIdentification { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string StudentCard { get; set; }
    public string FaceBook { get; set; }
    public string Instagram { get; set; }
    public string LinkedIn { get; set; }
    public string SelfDescription { get; set; }
    public string FavoriteMaxim { get; set; }
    public string Skill { get; set; }
    public string Experience { get; set; }
    public string Advantage { get; set; }
    public string Disadvantage { get; set; }
    public string CareerOrientation { get; set; }
    public string DesireCareer { get; set; }
    public Dictionary<string, object> SupervisorInfo { get; set; }    
    public StringEnum.AccountStatus AccountStatus { set; get; }
    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }
    public string Avatar { get; set; }
    public bool IsInternal { get; set; }
}