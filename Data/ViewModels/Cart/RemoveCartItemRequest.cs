using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.ViewModels.Cart
{
    public class RemoveCartItemRequest
    {
        public string CartId { set; get; }
        public string UserId { set; get; }
    }
}
