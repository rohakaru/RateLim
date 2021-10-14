using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            bool pass = false;

            if (pass)
            {
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
