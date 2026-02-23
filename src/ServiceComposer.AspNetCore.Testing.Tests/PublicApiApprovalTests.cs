using PublicApiGenerator;
using VerifyXunit;

namespace ServiceComposer.AspNetCore.Testing.Tests;

public class PublicApiApprovalTests
{
    [Fact]
    public Task Public_api_is_approved()
    {
        var publicApi = typeof(SelfContainedWebApplicationFactoryWithHost<>).Assembly.GeneratePublicApi();
        return Verifier.Verify(publicApi);
    }
}
