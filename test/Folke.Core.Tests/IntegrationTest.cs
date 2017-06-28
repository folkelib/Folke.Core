using System;
using System.Net.Http;
using System.Threading.Tasks;
using Folke.Core.Entities;
using Folke.Elm;
using Folke.Elm.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Folke.Core.Tests
{
    public class IntegrationTest
    {
        private readonly HttpClient client;

        public IntegrationTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<SampleStartup>());
            client = server.CreateClient();
        }

        [Fact]
        public async Task CallApiAccountMe_ReturnsNotLogged()
        {
            // Act
            var response = await client.GetAsync("/api/account/me");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("{\"userName\":null,\"logged\":false,\"emailConfirmed\":false,\"email\":null,\"id\":0,\"hasPassword\":false}",
                responseString);
        }

        public class SampleStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddFolkeCore<SqliteDriver>(options =>
                {
                    options.Elm = elmOptions => elmOptions.ConnectionString = "Data Source=:memory:";
                });
            }

            public void Configure(IApplicationBuilder app, IFolkeConnection connection, IHostingEnvironment environment, RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationPartManager applicationPartManager)
            {
                app.UseFolkeCore(connection, environment, roleManager, userManager, applicationPartManager,
                    options =>
                    {
                        options.AdministratorEmail = "admin@admin.com";
                        options.AdministratorPassword = "mypassword";
                    });
            }
        }
    }
}
