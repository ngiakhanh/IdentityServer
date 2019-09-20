using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Marvin.IDP
{
    public class Config
    {
        public static List<TestUser> GetUsers(){
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "123-123-123",
                    Username = "Frank",
                    Password = "123",
                    Claims = new List<Claim>
                    {
                        new Claim("first_name", "Frank"),
                        new Claim("family_name", "Underwood")
                    }
                },
                new TestUser
                {
                    SubjectId = "345-345-345",
                    Username = "Claire",
                    Password = "123",
                    Claims = new List<Claim>
                    {
                        new Claim("first_name", "Claire"),
                        new Claim("family_name", "Underwood")
                    }
                }
            };
        }

        // identity-related resources
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            { 
                new Client
                {
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44323/signin-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowOfflineAccess = true
                }, 
                new Client
                {
                    ClientName = "Intro",
                    ClientId = "intro",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44323/signin-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Client
                {
                    ClientName = "client",
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44323/signin-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }
    }
}