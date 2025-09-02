using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace GeekShopping.IdentityServer.Configuration
{
    public static class IdentityConfiguration
    {
        public const string Admin = "Admin";
        public const string Client = "Client";

        public static IEnumerable<IdentityResource> IdentityResources() => new List<IdentityResource>() {
           new IdentityResources.OpenId(),
           new IdentityResources.Email(),
           new IdentityResources.Profile()
        };

        public static IEnumerable<ApiScope> ApiScopes() => new List<ApiScope>() {
           new ("geek_shopping", "GeekShopping Server"),
           new (name: "read",   displayName: "Read data."),
           new (name: "write",  displayName: "Write data."),
           new (name: "delete", displayName: "Delete data.")
        };

        public static IEnumerable<Client> Clients() => new List<Client>() {
           new Client
           {
               ClientId = "client",
               ClientSecrets = { new Secret("teste".Sha256())},
               AllowedGrantTypes = GrantTypes.ClientCredentials,
               AllowedScopes = { "read", "write", "profile" }
           },
           new Client
           {
               ClientId = "geek_shopping",
               ClientSecrets = { new Secret("teste".Sha256())},
               AllowedGrantTypes = GrantTypes.Code,
               RedirectUris = { "https://localhost:4430/signin-oidc" },
               PostLogoutRedirectUris = { "https://localhost:4430/signout-callback-oidc" },
               AllowedScopes = new List<string>
               {
                   IdentityServerConstants.StandardScopes.OpenId,
                   IdentityServerConstants.StandardScopes.Email,
                   IdentityServerConstants.StandardScopes.Profile,
                   "geek_shopping"
               }
           }
        };
    }
}