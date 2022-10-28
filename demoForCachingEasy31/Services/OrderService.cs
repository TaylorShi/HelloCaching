using demoForTeslaOrder;
using EasyCaching.Core.Interceptor;
using System;
using System.Threading.Tasks;

namespace demoForCachingEasy31.Services
{
    public interface IOrderService
    {
        //[EasyCachingAble(Expiration = 30, CacheKeyPrefix = "GetOrder-", CacheProviderName = "DefaultRedis")]
        [EasyCachingAble(Expiration = 30)]
        Task<OrderModel> GetOrderAsync(string id);
    }

    public class OrderService : IOrderService
    {
        public Task<OrderModel> GetOrderAsync(string id)
        {
            return Task.FromResult(new OrderModel { Id = id, Date = DateTime.Now });
        }
    }
}
