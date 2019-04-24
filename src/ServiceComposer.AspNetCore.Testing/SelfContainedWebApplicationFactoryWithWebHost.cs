using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ServiceComposer.AspNetCore.Testing
{
    /// <summary>
    /// Factory for bootstrapping an application in memory for functional end to end tests.
    /// </summary>
    /// <typeparam name="TEntryPoint">
    /// A type in the entry point assembly of the application. Typically the Startup
    /// or Program classes can be used. Really whatever type in the entry assembly, 
    /// even the test fixture class works just fine.
    /// </typeparam>
    public class SelfContainedWebApplicationFactoryWithWebHost<TEntryPoint> :
        WebApplicationFactory<TEntryPoint> 
        where TEntryPoint : class
    {
        readonly Action<IServiceCollection> _configureServices;
        readonly Action<IApplicationBuilder> _configure;

        public SelfContainedWebApplicationFactoryWithWebHost(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
        {
            _configureServices = configureServices;
            _configure = configure;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(_configureServices)
                .Configure(_configure);

            return host;
        }
    }
}
