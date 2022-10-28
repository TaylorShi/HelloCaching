using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System.Text;
using System.Threading.Tasks;

namespace demoForCachingObjectPool31.Controllers
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
        public async Task<string> GetOrder([FromServices] ObjectPool<StringBuilder> builderPool, [FromServices]ILogger<OrderController> logger, [FromQuery] string id)
        {
            // 从对象池中获取StringBuilder对象
            var stringBuilder = builderPool.Get();
            for (int i = 0; i < 10000; i++)
            {
                // 从对象池中获取StringBuilder对象
                var stringBuilderItem = builderPool.Get();
                stringBuilderItem.Append(id);
                stringBuilder.Append(stringBuilderItem.ToString());
            }
            return await Task.FromResult(stringBuilder.ToString());
        }
    }
}
