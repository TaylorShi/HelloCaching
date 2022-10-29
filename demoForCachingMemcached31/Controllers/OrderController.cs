using demoForTeslaOrder;
using Enyim.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace demoForCachingMemcached31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrder([FromServices] IMemcachedClient memcachedClient, [FromServices] ILogger<OrderController> logger, [FromQuery] string id)
        {
            var key = $"GetOrder-{id ?? ""}";
            var order = await memcachedClient.GetValueOrCreateAsync(key, cacheSeconds:30, async () => 
            {
                return await Task.FromResult(new OrderModel
                {
                    Id = id,
                    Date = DateTime.Now
                });
            });
            return await Task.FromResult(Ok(order));
        }

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="logger"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromServices] IDistributedCache distributedCache, [FromServices] ILogger<OrderController> logger, [FromQuery] string id)
        {
            // 生成Key
            var key = $"GetOrders-{id ?? ""}";

            List<OrderModel> orders = null;
            // 根据Key获取缓存值
            var orderJson = await distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(orderJson))
            {
                orders = new List<OrderModel>() 
                { 
                    new OrderModel
                    {
                        Id = id,
                        Date = DateTime.Now
                    }
                };
                var distributedCacheOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                };
                orderJson = JsonConvert.SerializeObject(orders);
                // 根据Key写入缓存值
                await distributedCache.SetStringAsync(key, orderJson, distributedCacheOptions);
            }
            else
            {
                try
                {
                    orders = JsonConvert.DeserializeObject<List<OrderModel>>(orderJson);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "DeserializeObject OrderJson Failure");
                }
            }
            return Ok(orders);
        }
    }
}
