using FluentAssertions;
using IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace ChatPLC_Tests
{
    public class TestIdentityServer
    {
        [Fact]
        public void Check_If_TestUser_Exists()
        {
            var bob = IdentityServer.TestUsers.Users.SingleOrDefault(u => u.Username == "bob");
            Assert.NotNull(bob);
            Assert.Equal("bob", bob!.Password);
        }

        [Fact]
        public void ConfigureServices_RegistersIdentityServer()
        {
            var builder = WebApplication.CreateBuilder();
            builder.ConfigureServices();
            var provider = builder.Services.BuildServiceProvider();

            provider.GetRequiredService<IServer>()         // IdentityServer marker
                .Should().NotBeNull();
        }
    }
}
