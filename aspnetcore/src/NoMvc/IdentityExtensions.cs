using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;

namespace NoMvc
{
    public static class IdentityExtensions
    {
        public static void AddEmbeddedIdentityServer(this IServiceCollection s)
        {
            // set up embedded identity server
            s.AddIdentityServer().
                AddTestClients().
                AddTestResources().
                AddDeveloperSigningCredential();

            s.AddAuthorization(options =>
            {
                // set up authorization policy for the API
                options.AddPolicy("API", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser().RequireClaim("scope", "read");
                });
            })
            .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.Authority = "https://localhost:5001/identity";
            });
        }
    }

    public static class IdentityServerBuilderTestExtensions
    {
        public static IIdentityServerBuilder AddTestClients(this IIdentityServerBuilder builder)
        {
            return builder.AddInMemoryClients(new[] { new Client
            {
                ClientId = "client1",
                ClientSecrets =
                {
                    new Secret("secret1".Sha256())
                },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials
                },
                AllowedScopes = new[]
                {
                    "read"
                }
            }});
        }

        public static IIdentityServerBuilder AddTestResources(this IIdentityServerBuilder builder)
        {
            return builder.AddInMemoryApiResources(new[]
            {
                new ApiResource("embedded")
                {
                    Scopes =
                    {
                        new Scope("read")
                    },
                    Enabled = true
                },
            });
        }
    }
}