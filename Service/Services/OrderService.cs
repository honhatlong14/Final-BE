using Common.Constants;
using Data.Entities;
using Data.Entities.User;
using Data.IRepository;
using Data.ViewModels.OrderDetail;
using Data.ViewModels.Stall;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Service.IServices;

namespace Service.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<OrderDetail> _orderDetailRepo;
    private readonly IRepository<Book> _bookRepo;
    private readonly IRepository<Cart> _cartRepo;
    private readonly IRepository<Stall> _stallRepo;
    private readonly IRepository<Image> _imageRepo;
    private readonly IRepository<Address> _addressRepo;



    public OrderService(IRepository<Order> orderRepo, IRepository<Book> bookRepo,
        IRepository<Cart> cartRepo, IRepository<Stall> stallRepo, IRepository<Image> imageRepo,
        IRepository<OrderDetail> orderDetailRepo, IRepository<Address> addressRepo)
    {
        _orderRepo = orderRepo;
        _bookRepo = bookRepo;
        _cartRepo = cartRepo;
        _stallRepo = stallRepo;
        _imageRepo = imageRepo;
        _orderDetailRepo = orderDetailRepo;
        _addressRepo = addressRepo; 
    }

    public async Task<IEnumerable<Order>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Order> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> CreateOrderCashPayment(string userId)
    {
        var order = new Order();
        var orderDetails = new List<OrderDetail>();

        order.PaymentMethod = StringEnum.PaymentMethod.Cash;
        order.ShippingStatus = StringEnum.ShippingStatus.Prepare;
        order.UserId = userId;
        order.CreateAt = DateTime.UtcNow;
        order.CreatedBy = userId;
        var address = await _addressRepo.GetFirstOrDefaultAsync(a => (a.UserId == userId && a.AddressStatus == StringEnum.AddressStatus.Default)); 
        order.StreetName = address.StreetName;
        order.StreetNumber = address.StreetNumber;
        order.City = address.City;
        order.Country = address.Country; 
        
        var cartItem = await _cartRepo.GetAllAsync(c =>
            c.UserId == userId && c.IsSelected == StringEnum.SelectedItemStatus.Selected);
        foreach (var item in cartItem)
        {
            var orderDetail = new OrderDetail()
            {
                OrderId = order.Id,
                BookId = item.BookId,
                Quantity = item.Quantity,
                StallId = item.StallId,
                Total = item.Total
            };
            orderDetails.Add(orderDetail);
            _cartRepo.Remove(item);
            await _cartRepo.SaveChangesAsync();
        }

        order.OrderDetails = orderDetails;
        await _orderRepo.AddAsync(order);
        await _orderRepo.SaveChangesAsync();


        return order;
    }


    public async Task<Order> Update(string id, string userId, Cart model)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> UpdateShippingStatus(string id, string userId, int shippingStatus)
    {
        var order = await _orderRepo.GetFirstOrDefaultAsync(x => x.Id == id);
        order.ShippingStatus = (StringEnum.ShippingStatus?)shippingStatus; 
        _orderRepo.Update(order);
        await _orderRepo.SaveChangesAsync();
        return order;
    }

    public async Task Delete(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Order>> GetByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GetOrderResponse>> GetOrdersByUserId(string userId)
    {
        var listOrdersResponse = new List<GetOrderResponse>();
        var orders = await _orderRepo.GetAllAsync(o =>
                o.UserId == userId && (o.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.PaymentStatus == null), null,
            incl => (incl.OrderDetails));

        foreach (var orderItem in orders)
        {
            var order = new GetOrderResponse();
            var orderDetailsResponse = new List<GetOrderDetailsResponse>();

            order.OrderId = orderItem.Id; 
            order.PaymentMethod = orderItem.PaymentMethod;
            order.ShippingStatus = orderItem.ShippingStatus;
            order.PaymentStatus = orderItem.PaymentStatus;
            order.PaymentId = orderItem.PaymentId;
            order.UserId = orderItem.UserId;
            foreach (var item in orderItem.OrderDetails)
            {
                var orderDetail = new GetOrderDetailsResponse();
                orderDetail.OrderDetail = item;
                orderDetail.OrderDetail.Book = await _bookRepo.GetFirstOrDefaultAsync(b => b.Id == item.BookId);
                orderDetail.BookImageUrl =
                    (await _imageRepo.GetFirstOrDefaultAsync(i => i.BookId == item.BookId)).ImageUrl;
                orderDetail.StallName = (await _stallRepo.GetFirstOrDefaultAsync(s => s.Id == item.StallId)).StallName;

                orderDetailsResponse.Add(orderDetail);
            }

            order.OrderDetailsResponses = orderDetailsResponse;
            listOrdersResponse.Add(order);
        }

        return listOrdersResponse;
    }

    public async Task<string> GetOrdersByStallId(string stallId, string filter)
    {
        string jsonData = "";
        if (filter == "all")
        {
            var orders = (await _orderDetailRepo.GetAllAsync(o =>
                        o.StallId == stallId &&
                        (o.Order.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.Order.PaymentStatus == null),
                    orderBy => orderBy.OrderByDescending(o => o.CreateAt), include => include.Book.Images,
                    i => i.Order))
                .GroupBy(g => g.OrderId)
                .Select(group => new
                {
                    OrderId = group.Key,
                    PaymentId = group.First().Order.PaymentId,
                    PaymentMethod = group.First().Order.PaymentMethod,
                    PaymentStatus = group.First().Order.PaymentStatus,
                    ShippingStatus = group.First().Order.ShippingStatus,
                    OrderDetail = group.Select(o => new
                        { o.Book.Title, o.Quantity, o.Total, o.Book.Images.First().ImageUrl })
                });

            jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        }

        if (filter == "prepare")
        {
            var orders = (await _orderDetailRepo.GetAllAsync(o =>
                        o.StallId == stallId &&
                        (o.Order.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.Order.PaymentStatus == null) &&
                        o.Order.ShippingStatus == StringEnum.ShippingStatus.Prepare,
                    orderBy => orderBy.OrderByDescending(o => o.CreateAt), include => include.Book.Images,
                    i => i.Order))
                .GroupBy(g => g.OrderId)
                .Select(group => new
                {
                    OrderId = group.Key,
                    PaymentId = group.First().Order.PaymentId,
                    PaymentMethod = group.First().Order.PaymentMethod,
                    PaymentStatus = group.First().Order.PaymentStatus,
                    ShippingStatus = group.First().Order.ShippingStatus,
                    OrderDetail = group.Select(o => new
                        { o.Book.Title, o.Quantity, o.Total, o.Book.Images.First().ImageUrl })
                });

            jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        }

        if (filter == "onDelivery")
        {
            var orders = (await _orderDetailRepo.GetAllAsync(o =>
                        o.StallId == stallId &&
                        (o.Order.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.Order.PaymentStatus == null) &&
                        o.Order.ShippingStatus == StringEnum.ShippingStatus.OnDelivery,
                    orderBy => orderBy.OrderByDescending(o => o.CreateAt), include => include.Book.Images,
                    i => i.Order))
                .GroupBy(g => g.OrderId)
                .Select(group => new
                {
                    OrderId = group.Key,
                    PaymentId = group.First().Order.PaymentId,
                    PaymentMethod = group.First().Order.PaymentMethod,
                    PaymentStatus = group.First().Order.PaymentStatus,
                    ShippingStatus = group.First().Order.ShippingStatus,
                    OrderDetail = group.Select(o => new
                        { o.Book.Title, o.Quantity, o.Total, o.Book.Images.First().ImageUrl })
                });
            jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        }
        if (filter == "received")
        {
            var orders = (await _orderDetailRepo.GetAllAsync(o =>
                        o.StallId == stallId &&
                        (o.Order.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.Order.PaymentStatus == null) &&
                        o.Order.ShippingStatus == StringEnum.ShippingStatus.Received,
                    orderBy => orderBy.OrderByDescending(o => o.CreateAt), include => include.Book.Images,
                    i => i.Order))
                .GroupBy(g => g.OrderId)
                .Select(group => new
                {
                    OrderId = group.Key,
                    PaymentId = group.First().Order.PaymentId,
                    PaymentMethod = group.First().Order.PaymentMethod,
                    PaymentStatus = group.First().Order.PaymentStatus,
                    ShippingStatus = group.First().Order.ShippingStatus,
                    OrderDetail = group.Select(o => new
                        { o.Book.Title, o.Quantity, o.Total, o.Book.Images.First().ImageUrl })
                });

            jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        }
        return jsonData;
    }

    public async Task<IEnumerable<GetOrderDetailsResponse>> GetAllByUserId(string userId)
    {
        var orderDetailsResponse = new List<GetOrderDetailsResponse>();
        var orders = await _orderRepo.GetAllAsync(o =>
                o.UserId == userId && (o.PaymentStatus == StringEnum.PAYMENT_SUCCESS || o.PaymentStatus == null), null,
            incl => (incl.OrderDetails));

        foreach (var orderItem in orders)
        {
            foreach (var item in orderItem.OrderDetails)
            {
                var orderDetail = new GetOrderDetailsResponse();
                orderDetail.OrderDetail = item;
                orderDetail.OrderDetail.Book = await _bookRepo.GetFirstOrDefaultAsync(b => b.Id == item.BookId);
                orderDetail.BookImageUrl =
                    (await _imageRepo.GetFirstOrDefaultAsync(i => i.BookId == item.BookId)).ImageUrl;
                orderDetail.StallName = (await _stallRepo.GetFirstOrDefaultAsync(s => s.Id == item.StallId)).StallName;

                orderDetailsResponse.Add(orderDetail);
            }
        }

        return orderDetailsResponse;
    }

    public async Task<Order> RemoveByUserId(string id, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Order> CreateOrderOrderDetail(string userId, string paymentId, StringEnum.PaymentMethod paymentMethod)
    {
        var orderInstance = new Order();
        var orderDetails = new List<OrderDetail>();
        var address = await _addressRepo.GetFirstOrDefaultAsync(a => (a.UserId == userId && a.AddressStatus == StringEnum.AddressStatus.Default)); 

        orderInstance.UserId = userId;
        orderInstance.CreateAt = DateTime.UtcNow;
        orderInstance.CreatedBy = userId;
        orderInstance.PaymentStatus = StringEnum.PAYMENT_REQUIRED_METHOD;
        orderInstance.PaymentId = paymentId;
        orderInstance.ShippingStatus = StringEnum.ShippingStatus.Prepare;
        orderInstance.PaymentMethod = paymentMethod;

        var cartItem = await _cartRepo.GetAllAsync(c =>
            c.UserId == userId && c.IsSelected == StringEnum.SelectedItemStatus.Selected);
        foreach (var item in cartItem)
        {
            var orderDetail = new OrderDetail()
            {
                OrderId = orderInstance.Id,
                BookId = item.BookId,
                Quantity = item.Quantity,
                StallId = item.StallId,
                Total = item.Total
            };
            orderDetails.Add(orderDetail);
        }

        orderInstance.StreetName = address.StreetName;
        orderInstance.StreetNumber = address.StreetNumber;
        orderInstance.City = address.City;
        orderInstance.Country = address.Country; 

        orderInstance.OrderDetails = orderDetails;
        await _orderRepo.AddAsync(orderInstance);
        await _orderRepo.SaveChangesAsync();
        return orderInstance;
    }

    public async Task<string> GetTotalOrder()
    {
        var orders = (await _orderRepo.GetAllAsync()).Count(); 
        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );

        return jsonData;
    }
}