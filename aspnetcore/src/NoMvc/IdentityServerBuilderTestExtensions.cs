﻿using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NoMvc
{
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