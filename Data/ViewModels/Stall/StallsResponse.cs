using Common.Constants;

namespace Data.ViewModels.Stall;

public class StallsResponse
{
    public string Id { set; get; }
    public string UserId { set; get; }
    public string StallName { set; get; }
    public string FullName { set; get; }
    public string? PhoneNumber { get; set; }
    public StringEnum.StallStatus StallStatus { set; get; }
}