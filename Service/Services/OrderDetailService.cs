using System.Runtime.CompilerServices;
using Data.Entities;
using Data.IRepository;
using Newtonsoft.Json;
using Service.IServices;

namespace Service.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IRepository<OrderDetail> _orderDetailRepo;
    private readonly IRepository<Book> _bookRepo;

    public OrderDetailService(IRepository<OrderDetail> orderDetailRepo, IRepository<Book> bookRepo)
    {
        _orderDetailRepo = orderDetailRepo;
        _bookRepo = bookRepo;
    }

    public async Task<string> GetProfit(string stallId)
    {
        var now = DateTime.Now;
        var threeDaysAgo = now.AddDays(-10);

        var profit = (await _orderDetailRepo.GetAllAsync(o => o.StallId == stallId, o => o.OrderByDescending(o => o.CreateAt) , i => i.Book))
            .Where(p => p.CreateAt >= threeDaysAgo && p.CreateAt <= now).GroupBy(g => g.CreateAt.Value.Date).Select(
                group => new
                {
                    DateTime = group.Key.Date,
                    BookDataInfo = group.Select(p => new { p.BookId, p.CreateAt, p.Total, p.Book.Title }).GroupBy(g => g.BookId)
                        .Select(
                            group => new
                            {
                                BookId = group.Key,
                                BookTitle = group.First().Title, 
                                TotalProfit = group.Sum(s => s.Total)
                            })
                });
        

        string jsonData = JsonConvert.SerializeObject(profit, Formatting.Indented);
        return jsonData;
    }

    public async Task<string> GetProfitByQuantity(string stallId)
    {
        var now = DateTime.Now;
        var threeDaysAgo = now.AddDays(-10);
        var profit = (await _orderDetailRepo.GetAllAsync(o => o.StallId == stallId,
                o => o.OrderByDescending(o => o.CreateAt), i => i.Book))
            .Where(p => p.CreateAt >= threeDaysAgo && p.CreateAt <= now).GroupBy(g => g.CreateAt.Value.Date).Select(g =>
                new
                {
                    DateTime = g.Key.Date,
                    QuantityBookSold = g.Sum(x => x.Quantity)
                }); 
        string jsonData = JsonConvert.SerializeObject(profit, Formatting.Indented);
        return jsonData;
    }


    public async Task<string> GetByOrderIdStallId(string orderId, string stallId)
    {
        var data = (await _orderDetailRepo.GetAllAsync(o => o.Order.Id == orderId && o.StallId == stallId, null,
            i => i.Order, i => i.Book.Images, i => i.Order.User))
            .GroupBy(g => g.StallId).Select(g => new
            {
                orderId = g.First().OrderId, 
                fullName = g.First().Order.User.FullName, 
                phoneNumber = g.First().Order.User.PhoneNumber, 
                stallId = g.Key,
                paymentMethod = g.First().Order.PaymentMethod,
                paymentStatus = g.First().Order.PaymentStatus,
                shippingStatus = g.First().Order.ShippingStatus,
                streetNumber = g.First().Order.StreetNumber, 
                streetName = g.First().Order.StreetName, 
                city = g.First().Order.City, 
                country = g.First().Order.Country, 
                orderDetailData = g.Select(o => new
                {
                    bookId = o.BookId,
                    quantity = o.Quantity,
                    total = o.Total,
                    bookImageUrl = o.Book.Images.First().ImageUrl,
                    bookTitle = o.Book.Title  // Thêm dòng này để lấy thông tin title của từng sách
                })
                
            });
        
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented,
            new JsonSerializerSettings() {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );
        // string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        return jsonData; 
    }

    public async Task<string> GetByOrderId(string orderId)
    {
        var data = (await _orderDetailRepo.GetAllAsync(o => o.Order.Id == orderId, null,
                i => i.Order, i => i.Book.Images, i => i.Order.User))
            .GroupBy(g => g.OrderId).Select(g => new
            {
                orderId = g.Key, 
                fullName = g.First().Order.User.FullName, 
                phoneNumber = g.First().Order.User.PhoneNumber, 
                stallId = g.First().StallId,
                paymentMethod = g.First().Order.PaymentMethod,
                paymentStatus = g.First().Order.PaymentStatus,
                shippingStatus = g.First().Order.ShippingStatus,
                streetNumber = g.First().Order.StreetNumber, 
                streetName = g.First().Order.StreetName, 
                city = g.First().Order.City, 
                country = g.First().Order.Country, 
                orderDetailData = g.Select(o => new
                {
                    bookId = o.BookId,
                    quantity = o.Quantity,
                    total = o.Total,
                    bookImageUrl = o.Book.Images.First().ImageUrl,
                    bookTitle = o.Book.Title  // Thêm dòng này để lấy thông tin title của từng sách
                })
                
            });
        
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented,
            new JsonSerializerSettings() {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );
        // string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        return jsonData; 
    }

    public async Task<string> GetProfitTotalIncome()
    {
        var orderDetails = (await _orderDetailRepo.GetAllAsync());
        
        decimal totalProfit = orderDetails.Sum(g => g.Total);
        
        var jsonData = JsonConvert.SerializeObject(totalProfit, Formatting.Indented,
            new JsonSerializerSettings() {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );
        // string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        return jsonData; 
    }
    

    public async Task<string> GetTopUserStall(string stallId)
    {
        var topUsers = (await _orderDetailRepo.GetAllAsync(x => x.StallId == stallId, null, o => o.Order, o => o.Order.User))
            .GroupBy(od => od.Order.UserId)
            .Select(g => new
            {
                userId = g.Key,
                total = g.Sum(od => od.Total),
                phoneNumber = g.First().Order.User.PhoneNumber,
                fullName = g.First().Order.User.FullName,
                avatar = g.First().Order.User.Avatar
            })
            .OrderByDescending(u => u.total)
            .Take(3);

        var jsonData = JsonConvert.SerializeObject(topUsers, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );
        return jsonData;
    }
    
    public async Task<string> GetTopUser()
    {
        var topUsers = (await _orderDetailRepo.GetAllAsync(null, null, o => o.Order, o => o.Order.User))
            .GroupBy(od => od.Order.UserId)
            .Select(g => new
            {
                userId = g.Key,
                total = g.Sum(od => od.Total),
                phoneNumber = g.First().Order.User.PhoneNumber,
                fullName = g.First().Order.User.FullName,
                avatar = g.First().Order.User.Avatar
            })
            .OrderByDescending(u => u.total)
            .Take(3);

        var jsonData = JsonConvert.SerializeObject(topUsers, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }
        );
        return jsonData;
    }

}