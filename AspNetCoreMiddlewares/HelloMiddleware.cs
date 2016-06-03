using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMiddlewares
{
    public class HelloMiddleware
    {
        private RequestDelegate _next;

        public HelloMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync("Hello Starts\r\n");

            await _next(context);

            await context.Response.WriteAsync("Hello Ends\r\n");
        }
    }
}
