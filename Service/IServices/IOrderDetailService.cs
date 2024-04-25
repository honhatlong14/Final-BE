using Data.Entities;
using Data.IRepository;

namespace Service.IServices;

public interface IOrderDetailService
{
    Task<string> GetProfit(string stallId);
    Task<string> GetProfitByQuantity(string stallId);
    Task<string> GetByOrderIdStallId(string orderId, string stallId); 
    Task<string> GetByOrderId(string orderId); 
    Task<string> GetProfitTotalIncome(); 
    Task<string> GetTopUser(); 
    Task<string> GetTopUserStall(string stallId); 
}
