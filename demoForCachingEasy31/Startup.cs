using demoForCachingEasy31.Services;
using EasyCaching.Core.Configurations;
using EasyCaching.Interceptor.AspectCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoForCachingEasy31
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddScoped<IOrderService, OrderService>();

            //// 添加EasyCaching服务
            //services.AddEasyCaching(easyCachingOptions =>
            //{
            //    easyCachingOptions.UseRedis(Configuration, name: "DefaultRedis").WithJson("redis");
            //    easyCachingOptions.UseRedis(Configuration, name: "EasyCaching").WithJson("redis");
            //});
            //services.ConfigureAspectCoreInterceptor(options =>
            //{
            //    options.CacheProviderName = "EasyCaching";
            //});

            services.AddEasyCaching(easyCachingOptions => 
            {
                easyCachingOptions.UseInMemory("m1");
                easyCachingOptions.UseRedis(Configuration, name: "EasyCaching").WithJson("redis");

                easyCachingOptions.UseHybrid(options =>
                {
                    options.EnableLogging = true;
                    // 缓存总线的订阅主题
                    options.TopicName = "test_topic";
                    // 本地缓存的名字
                    options.LocalCacheProviderName = "m1";
                    // 分布式缓存的名字
                    options.DistributedCacheProviderName = "EasyCaching";
                });
                easyCachingOptions.WithRedisBus(Configuration, name: "DefaultRedis");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
