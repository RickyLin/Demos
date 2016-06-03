using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMiddlewares
{
    public class WorldMiddleware
    {
        private RequestDelegate _next;

        public WorldMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync("World Starts\r\n");

            await _next(context);

            await context.Response.WriteAsync("World Ends\r\n");
        }
    }
}
