
using System.Security.Claims;
using ChatPLC_Backend.Helpers;
using ChatPLC_Backend.Services;
using ChatPLC_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChatPLC_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // builder.Services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowFrontendOrigin",
            //         policy =>
            //         {
            //             policy.WithOrigins("https://localhost:3000")
            //                 .AllowAnyHeader()
            //                 .AllowAnyMethod()
            //                 .AllowCredentials();
            //         });
            // });

            // Add services to the container.
            builder.Services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.TokenValidationParameters.ValidateAudience = false;
                });
            
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api");
                });
            });
            
            builder.Services.AddHttpClient<IRagWrapper, RagWrapper>(client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            });
            builder.Services.AddHttpClient<IAnthropicWrapper, AnthropicWrapper>(client =>
            {
                client.BaseAddress = new Uri("https://api.anthropic.com/v1/");
            });
            // builder.Services.AddScoped<IRagWrapper, RagWrapper>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }
            
            app.UseHttpsRedirection();

            app.UseAuthorization();

            // app.UseCors("AllowFrontendOrigin");
            app.MapGet("identity", (ClaimsPrincipal user) => user.Claims.Select(c => new { c.Type, c.Value }))
                .RequireAuthorization("ApiScope");
            
            app.MapControllers();

            app.Run();
        }
    }
}
