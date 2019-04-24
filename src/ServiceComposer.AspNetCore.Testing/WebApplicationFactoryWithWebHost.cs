using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ServiceComposer.AspNetCore.Testing
{
    public class WebApplicationFactoryWithWebHost<TStartup> :
        WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<TStartup>();

            return host;
        }
    }
}
