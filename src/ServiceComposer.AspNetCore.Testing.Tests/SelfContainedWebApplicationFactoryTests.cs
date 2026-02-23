using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceComposer.AspNetCore.Testing.Tests;

public class SelfContainedWebApplicationFactoryTests
{
    [Fact]
    public async Task SelfContainedFactoryWithHost_serves_request()
    {
        await using var factory = new SelfContainedWebApplicationFactoryWithHost<TestEntryPoint>(
            services => services.AddSingleton(new Message("host")),
            app => app.Run(async context =>
            {
                var message = context.RequestServices.GetRequiredService<Message>();
                await context.Response.WriteAsync(message.Value);
            }));

        var client = factory.CreateClient();
        var response = await client.GetStringAsync("/");

        Assert.Equal("host", response);
    }

    [Fact]
    public async Task SelfContainedFactoryWithWebHost_invokes_builder_customization()
    {
        var builderCustomizationCalled = false;
        await using var factory = new SelfContainedWebApplicationFactoryWithWebHost<TestEntryPoint>(
            services => services.AddSingleton(new Message("webhost")),
            app => app.Run(async context =>
            {
                var message = context.RequestServices.GetRequiredService<Message>();
                await context.Response.WriteAsync(message.Value);
            }))
        {
            BuilderCustomization = _ => builderCustomizationCalled = true
        };

        var client = factory.CreateClient();
        var response = await client.GetStringAsync("/");

        Assert.Equal("webhost", response);
        Assert.True(builderCustomizationCalled);
    }

    class TestEntryPoint;

    class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(context => context.Response.WriteAsync("startup"));
        }
    }

    record Message(string Value);
}
