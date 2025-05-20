using Duende.Bff.Yarp;

namespace React.BFF;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontendOrigin",
                policy =>
                {
                    policy.WithOrigins("https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
        
        builder.Services
            .AddBff()
            .AddRemoteApis();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";
                
                options.ClientId = "bff";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("api");
                options.Scope.Add("offline_access"); // <-- When AllowOfflineAccess = true in identityServer for bff client
                
                options.SaveTokens = true; // <-- Save tokens in the cookie
                options.GetClaimsFromUserInfoEndpoint = true; // <-- Get claims from userinfo endpoint
                options.MapInboundClaims = false; // <-- Don't map claims from the id_token
                
            });

        builder.Services.AddAuthorization();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();

        app.UseBff();

        app.UseAuthorization();
        app.MapBffManagementEndpoints();
        app.UseCors("AllowFrontendOrigin");
        
        //ToDo: Dont forget to add urls that need to be proxied to the backend api.
        app.MapRemoteBffApiEndpoint("/test", "https://localhost:7001/test");
            // .RequireAccessToken(Duende.Bff.TokenType.User);

        app.MapFallbackToFile("/index.html");

        return app;
    }
}