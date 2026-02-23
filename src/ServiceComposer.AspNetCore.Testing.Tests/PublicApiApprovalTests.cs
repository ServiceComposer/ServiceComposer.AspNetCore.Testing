using PublicApiGenerator;
using VerifyXunit;

namespace ServiceComposer.AspNetCore.Testing.Tests;

public class PublicApiApprovalTests
{
    [Fact]
    public Task Public_api_is_approved()
    {
        var publicApi = ApiGenerator.GeneratePublicApi(typeof(WebApplicationFactoryWithWebHost<>).Assembly);
        return Verifier.Verify(publicApi);
    }
}
