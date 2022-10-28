using demoForTeslaOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace demoForCachingRedis31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrder([FromServices]IDistributedCache distributedCache, [FromServices]ILogger<OrderController> logger, [FromQuery] string id)
        {
            OrderModel order = null;
            var key = $"GetOrder-{id ?? ""}";
            var value = await distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
            {
                var option = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                };
                order = new OrderModel
                {
                    Id = id,
                    Date = DateTime.Now
                };
                await distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(order), option);
            }
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    order = JsonConvert.DeserializeObject<OrderModel>(value);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "反序列化失败");
                }
            }
            return Ok(order);
        }
    }
}
