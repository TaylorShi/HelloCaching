using demoForTeslaOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace demoForCachingMemory31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        ///// <summary>
        ///// 获取订单
        ///// 缓存过期时间6000秒，缓存Key的生成策略是基于id的值，不同id的值会缓存为不同的Cache
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[ResponseCache(Duration = 6000, VaryByQueryKeys = new string[] { "id" })]
        //[HttpGet]
        //public OrderModel GetOrder([FromQuery] string id)
        //{
        //    return new OrderModel { Id = id, Date = DateTime.Now };
        //}

        ///// <summary>
        ///// 获取地址
        ///// 缓存过期时间6000秒，缓存Key的生成策略是名为rpc的Header，这个Header不同的值就会缓存不同的Cache
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[ResponseCache(Duration = 6000, VaryByHeader = "rpc")]
        //[HttpGet]
        //public OrderModel GetAddress([FromQuery] string id)
        //{
        //    return new OrderModel { Id = id, Date = DateTime.Now };
        //}

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrder([FromServices]IMemoryCache memoryCache, [FromServices] ILogger<OrderController> logger, [FromQuery] string id)
        {
            OrderModel order;
            var key = $"GetOrder-{id ?? ""}";
            if(!memoryCache.TryGetValue(key, out order))
            {
                order = new OrderModel
                {
                    Id = id,
                    Date = DateTime.Now
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    // 滑动过期，多长时间不访问才失效，这里30秒，30秒不访问就失效
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                    // 绝对到期，在滑动过期间隔内未请求该项时，到了60秒仍然会自动过期
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                };
                memoryCache.Set(key, order, cacheEntryOptions);
            }
            return await Task.FromResult(Ok(order));
        }

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderV2([FromServices] IMemoryCache memoryCache, [FromServices] ILogger<OrderController> logger, [FromQuery] string id)
        {
            var key = $"GetOrder-{id ?? ""}";
            var order = await memoryCache.GetOrCreateAsync(key, cacheEntry => {

                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return Task.FromResult(new OrderModel
                {
                    Id = id,
                    Date = DateTime.Now
                });
            });
            return await Task.FromResult(Ok(order));
        }
    }
}
