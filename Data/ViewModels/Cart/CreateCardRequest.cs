using Common.Constants;

namespace Data.ViewModels.Cart;

public class CreateCardRequest
{
    public string UserId { set; get; }
    public string BookId { set; get; }
    public int Quantity { set; get; }
    public int Total { set; get; }
    public StringEnum.SelectedItemStatus IsSelected { set; get; }
}