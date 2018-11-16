using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace RuntimeControllers
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionDescriptorChangeProvider, OnDemandActionDescriptorChangeProvider>();
            services.AddSingleton<ApplicationPartWatcher>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationPartWatcher applicationPartWatcher)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();

            Task.Run(() => applicationPartWatcher.Watch("plugins"));
        }
    }
}
