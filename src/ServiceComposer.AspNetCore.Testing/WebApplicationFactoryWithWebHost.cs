using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace ServiceComposer.AspNetCore.Testing
{
    public class WebApplicationFactoryWithWebHost<TStartup> :
        WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public string Environment { get; set; } = Environments.Development;
        public Action<IWebHostBuilder> BuilderCustomization { get; set; }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(Array.Empty<string>())
                .UseEnvironment(Environment)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            BuilderCustomization?.Invoke(builder);
        }
    }
}
