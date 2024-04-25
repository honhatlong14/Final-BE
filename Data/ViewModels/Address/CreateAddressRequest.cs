using Common.Constants;

namespace Data.ViewModels.Address;

public class CreateAddressRequest
{
    public string StreetNumber { set; get; }
    public string StreetName { set; get; }
    public string City { set; get; }
    public string Country { set; get; }
    public string UserId { set; get; }
    // Enum: status Address ... 
    public StringEnum.AddressStatus AddressStatus { set; get; }    
}