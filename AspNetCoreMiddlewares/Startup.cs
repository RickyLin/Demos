using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMiddlewares
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<HelloMiddleware>();
            app.UseMiddleware<WorldMiddleware>();

            app.Use(next => async context =>
            {
                await context.Response.WriteAsync("12345\r\n");

                await next(context);
            });
        }
    }
}
