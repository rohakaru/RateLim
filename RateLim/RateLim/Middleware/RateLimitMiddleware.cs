using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLim.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer _redis;
        private readonly int Limit;
        private readonly int ExpireHour;
        private readonly int ExpireMinute;
        private readonly int ExpireSecond;

        public RateLimitMiddleware(RequestDelegate next, IConnectionMultiplexer redis, IConfiguration config)
        {
            _next = next;
            _redis = redis;
            Limit = Convert.ToInt32(config["Redis:Limit"]);
            ExpireHour = Convert.ToInt32(config["Redis:Expiry:Hour"]);
            ExpireMinute = Convert.ToInt32(config["Redis:Expiry:Minute"]);
            ExpireSecond = Convert.ToInt32(config["Redis:Expiry:Second"]);
        }

        public async Task Invoke(HttpContext context)
        {
            IDatabase data = _redis.GetDatabase();
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString().Replace(':', ';');//IPv6的冒號與redis分層符號相同，所以換成分號
            RedisValueWithExpiry result = await data.StringGetWithExpiryAsync(remoteIpAddress);

            int count = Convert.ToInt32(result.Value);
            bool pass = (count < Limit);

            if (pass)
            {
                await data.StringSetAsync(remoteIpAddress, ++count, result.Expiry ?? new TimeSpan(ExpireHour, ExpireMinute, ExpireSecond));
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }

            context.Response.Headers.Add("X-RateLimit-Remaining", (Limit - count).ToString());
            context.Response.Headers.Add("X-RateLimit-Reset", DateTime.Now.Add(result.Expiry ?? new TimeSpan(ExpireHour, ExpireMinute, ExpireSecond)).ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
