using demoForCachingEasy31.Services;
using demoForTeslaOrder;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace demoForCachingEasy31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEasyCachingProvider _provider;
        private readonly IEasyCachingProvider _defaultProvider;
        private readonly IEasyCachingProvider _easyProvider;

        //public OrderController(IEasyCachingProvider provider, IEasyCachingProviderFactory factory)
        //{
        //    this._provider = provider;
        //    //this._defaultProvider = factory.GetCachingProvider("DefaultRedis");
        //    //this._easyProvider = factory.GetCachingProvider("EasyCaching");
        //}

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetModel([FromQuery] string id)
        {
            var key = $"GetModel-{id ?? ""}";
            // 获取这个Key的值，Value直接返回当前时间，过期时间为60秒
            var value = _provider.Get(key, () => DateTime.Now.ToString(), TimeSpan.FromSeconds(60));
            return Content(value.Value);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetModelV2([FromQuery] string id)
        {
            var key = $"GetModel-{id ?? ""}";
            // 获取缓存key的值，没有默认值
            var value = await _provider.GetAsync<string>(key);
            // 获取缓存key的值，默认值返回当前时间，过期时间一分钟
            var value2 = await _provider.GetAsync(key, async () => await Task.FromResult(DateTime.Now.ToString()), TimeSpan.FromMinutes(1));
            // 设置缓存Key=demo，value=123，过期时间为一分钟
            await _provider.SetAsync("demo", "123", TimeSpan.FromMinutes(1));
            // 删除缓存key=demo
            await _provider.RemoveAsync("demo");
            return Content(value2.Value);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetModelV3([FromQuery] string id)
        {
            var key = $"GetModel-{id ?? ""}";
            // 获取缓存key的值，没有默认值
            var value = await _defaultProvider.GetAsync<string>(key);
            // 获取缓存key的值，默认值返回当前时间，过期时间一分钟
            var value2 = await _defaultProvider.GetAsync(key, async () => await Task.FromResult(DateTime.Now.ToString()), TimeSpan.FromMinutes(1));
            // 设置缓存Key=demo，value=123，过期时间为一分钟
            await _easyProvider.SetAsync("demo", "123", TimeSpan.FromMinutes(1));
            // 删除缓存key=demo
            //await _easyProvider.RemoveAsync("demo");
            return Content(value2.Value);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderV2([FromServices]IOrderService orderService, [FromQuery] string id)
        {
            var order = await orderService.GetOrderAsync(id);
            return Ok(order);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetHybird([FromServices] IHybridCachingProvider hybridCachingProvider, [FromQuery] string id)
        {
            var key = $"GetModel-{id ?? ""}";
            var value = await hybridCachingProvider.GetAsync<string>(key, async () => await Task.FromResult(DateTime.Now.ToString()), TimeSpan.FromMinutes(1));
            return Ok(value.Value);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="easyCachingProvider"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRedis([FromServices] IEasyCachingProviderFactory factory, [FromQuery] string id)
        {
            var key = $"GetModel-{id ?? ""}";
            var redisProvider = factory.GetRedisProvider("EasyCaching");
            var value = await redisProvider.StringGetAsync(key);
            if(value == null)
            {
                await redisProvider.StringSetAsync(key, DateTime.Now.ToString(), TimeSpan.FromMinutes(1));
                value = await redisProvider.StringGetAsync(key);
            }
            return Ok(value);
        }
    }
}
