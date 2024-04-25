using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;

namespace Data.ViewModels.OrderDetail;

public class GetOrderDetailsResponse
{
    public Entities.OrderDetail OrderDetail  { set; get; }
    public string StallName { set; get; }
    public string BookImageUrl { set; get; }

}