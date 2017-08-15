using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreMiddlewares
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<HelloMiddleware>();
            app.UseMiddleware<WorldMiddleware>();

            app.Run(async context => await context.Response.WriteAsync("12345\r\n"));
        }
    }
}
