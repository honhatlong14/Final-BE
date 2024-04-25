using Common.Constants;
using Data.ViewModels.OrderDetail;

namespace Data.ViewModels.Stall;

public class GetOrdersStallResponse
{
   public string UserId { set; get; }
    public string? PaymentId { set; get; }
    public StringEnum.PaymentMethod PaymentMethod { set; get; }
    public string? PaymentStatus { set; get; }
    public StringEnum.ShippingStatus? ShippingStatus { set; get; }
    public IList<GetOrderDetailsResponse> OrderDetailsResponses { set; get;  }

}