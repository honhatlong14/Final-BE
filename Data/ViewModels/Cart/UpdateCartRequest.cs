using Common.Constants;

namespace Data.ViewModels.Cart;

public class UpdateCartRequest
{
    public string UserId { set; get; }
    public int Quantity { set; get; }
    public StringEnum.SelectedItemStatus IsSelected { set; get; }
}