using Common.Constants;
using Data.Entities;
using Data.ViewModels.OrderDetail;
using Data.ViewModels.Stall;

namespace Service.IServices;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAll();
    Task<Order> GetById(string id);
    Task<Order> CreateOrderCashPayment(string userId);
    Task<Order> Update(string id, string userId, Cart model);
    Task<Order> UpdateShippingStatus(string id, string userId, int shippingStatus);
    
    Task Delete(string id);
    Task<IEnumerable<Order>> GetByUserId(string userId);
    Task<IEnumerable<GetOrderDetailsResponse>> GetAllByUserId(string userId);
    Task<IEnumerable<GetOrderResponse>> GetOrdersByUserId(string userId);
    Task<string> GetOrdersByStallId(string stallId, string filter); 
    Task<Order> RemoveByUserId(string id, string userId); 
    Task<Order> CreateOrderOrderDetail(string userId, string paymentId, StringEnum.PaymentMethod paymentMethod);
    Task<string> GetTotalOrder();
}