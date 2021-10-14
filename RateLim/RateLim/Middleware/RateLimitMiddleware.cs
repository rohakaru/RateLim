using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        private const int Limit = 1000;

        public RateLimitMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
        {
            _next = next;
            _redis = redis;
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
                await data.StringSetAsync(remoteIpAddress, ++count, result.Expiry ?? new TimeSpan(1, 0, 0));
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
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
