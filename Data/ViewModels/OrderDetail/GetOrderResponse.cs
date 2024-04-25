using Common.Constants;

namespace Data.ViewModels.OrderDetail;

public class GetOrderResponse
{
    public string OrderId { set; get; }
    public string UserId { set; get; }
    public string? PaymentId { set; get; }
    public StringEnum.PaymentMethod PaymentMethod { set; get; }
    public string? PaymentStatus { set; get; }
    public StringEnum.ShippingStatus? ShippingStatus { set; get; }
    public IList<GetOrderDetailsResponse> OrderDetailsResponses { set; get;  }

    
}