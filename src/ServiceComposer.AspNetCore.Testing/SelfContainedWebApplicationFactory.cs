using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ServiceComposer.AspNetCore.Testing
{
    public class SelfContainedWebApplicationFactory :
        WebApplicationFactory<SelfContainedWebApplicationFactory.Ignored>
    {
        public class Ignored
        {
            public void ConfigureServices(IServiceCollection services)
            {

            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
            {

            }
        }

        public Action<IServiceCollection> ConfigureServices { get; set; } = _ => { };
        public Action<IApplicationBuilder> Configure { get; set; } = _ => { };

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(ConfigureServices)
                .Configure(Configure);

            return host;
        }
    }
}
