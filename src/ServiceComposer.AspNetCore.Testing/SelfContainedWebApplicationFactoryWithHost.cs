﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceComposer.AspNetCore.Testing
{
    public class SelfContainedWebApplicationFactoryWithHost<TEntryPoint> :
        WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly Action<IServiceCollection> configureServices;
        private readonly Action<IApplicationBuilder> configure;
        private readonly string[] args;

        public Action<IHostBuilder> HostBuilderCustomization { get; set; }

        public Action<IWebHostBuilder> WebHostBuilderCustomization { get; set; }

        public SelfContainedWebApplicationFactoryWithHost(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure, string[] args = null)
        {
            this.configureServices = configureServices;
            this.configure = configure;
            this.args = args ?? new string[0];
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(configureServices);
                webBuilder.Configure(configure);

                WebHostBuilderCustomization?.Invoke(webBuilder);
            });

            HostBuilderCustomization?.Invoke(hostBuilder);

            return hostBuilder;
        }
    }
}