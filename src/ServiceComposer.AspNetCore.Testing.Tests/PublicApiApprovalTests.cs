using PublicApiGenerator;
using VerifyXunit;

namespace ServiceComposer.AspNetCore.Testing.Tests;

public class ApiApprovals
{
    [Fact]
    public Task Approve_API()
    {
        var publicApi = typeof(SelfContainedWebApplicationFactoryWithHost<>).Assembly.GeneratePublicApi(new ApiGeneratorOptions
            {
            ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
        })
        ;
        return Verifier.Verify(publicApi);
    }
}
