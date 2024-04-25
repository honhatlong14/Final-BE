using Common.Constants;
using Data.Entities.User;

namespace Data.ViewModels.Cart;

public class GetAllCartResponse
{
    public string Id { set; get; }
    public string UserId { set; get; }

    public Entities.User.User User { set; get; }

    // PK FK
    public string BookId { set; get; }
    public Entities.Book Book { set; get; }
    public int Quantity { set; get; }
    public int Total { set; get; }
    
    public string StallId { set; get; }
    public Entities.User.Stall Stall { set; get; }
    public string StallName { set; get; }
    public StringEnum.SelectedItemStatus IsSelected { set; get; }
}