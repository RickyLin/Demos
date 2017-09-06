using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.DependencyInjection;

namespace CustomizedViewLocation
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton<RazorTemplateEngine, ModuleRazorTemplateEngine>(); // don't need this any more if we make use of ModuleBasedRazorProject
            services.AddSingleton<RazorProject, ModuleBasedRazorProject>();

            IMvcBuilder mvcBuilder = services.AddMvc();
            mvcBuilder.AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/{1}/Views/{0}.cshtml");
                options.ViewLocationExpanders.Add(new NamespaceViewLocationExpander());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvcWithDefaultRoute();
        }
    }
}
