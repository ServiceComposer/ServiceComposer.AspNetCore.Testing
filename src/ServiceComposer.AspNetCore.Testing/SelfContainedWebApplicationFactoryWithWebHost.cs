using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ServiceComposer.AspNetCore.Testing
{
    /// <summary>
    /// Factory for bootstrapping an application in memory for functional end-to-end tests.
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
        readonly Action<WebHostBuilderContext, IServiceCollection> _configureServicesWithWebHostBuilderContext;
        readonly Action<WebHostBuilderContext, IApplicationBuilder> _configureWithWebHostBuilderContext;

        public string Environment { get; set; } = Environments.Development;
        public Action<IWebHostBuilder> BuilderCustomization { get; set; }

        public SelfContainedWebApplicationFactoryWithWebHost(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
        {
            _configureServices = configureServices;
            _configure = configure;
        }

        public SelfContainedWebApplicationFactoryWithWebHost(Action<WebHostBuilderContext, IServiceCollection> configureServices, Action<WebHostBuilderContext, IApplicationBuilder> configure)
        {
            _configureServicesWithWebHostBuilderContext = configureServices;
            _configureWithWebHostBuilderContext = configure;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(Array.Empty<string>())
                .UseEnvironment(Environment)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (_configureServices != null)
                    {
                        webBuilder.ConfigureServices(_configureServices);
                    }

                    if (_configureServicesWithWebHostBuilderContext != null)
                    {
                        webBuilder.ConfigureServices(_configureServicesWithWebHostBuilderContext);
                    }

                    if (_configure != null)
                    {
                        webBuilder.Configure(_configure);
                    }

                    if (_configureWithWebHostBuilderContext != null)
                    {
                        webBuilder.Configure(_configureWithWebHostBuilderContext);
                    }
                });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            BuilderCustomization?.Invoke(builder);
        }
    }
}