using Microsoft.Extensions.Configuration;

namespace LeaveManagement.Tests.TestHelpers;

public static class JwtConfigFactory
{
    public static IConfiguration Create() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"]        = "TestOnly$SuperSecret#Key2025!32Ch",
                ["JwtSettings:Issuer"]            = "TestIssuer",
                ["JwtSettings:Audience"]          = "TestAudience",
                ["JwtSettings:ExpiryInMinutes"]   = "60"
            })
            .Build();
}
