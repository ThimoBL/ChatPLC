using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer
{
    public static class Config
    {
        /// <summary>
        /// Identity resources are data structures that define the information that is requested from the user during authentication.
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        ];

        /// <summary>
        /// Api scopes are used to define the permissions that a client can request from the user.
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new(name: "api", displayName: "My API")
        ];

        /// <summary>
        /// Clients are applications that want to access your API. They can be web applications, mobile applications,
        /// or any other type of application.
        /// </summary>
        public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientId = "client",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "api" }
            },
            new()
            {
                ClientId = "bff",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "https://localhost:3000/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:3000/signout-callback-oidc" },
                
                AllowOfflineAccess = true, // <-- Claim for refresh token, allow offline access

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api"
                }
            }
        ];
    }
}