using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Folke.Elm;
using Folke.Elm.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Folke.Core.Entities;

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

        [Fact]
        public async Task LoginAsAdminThenCallApiAccountMe_ReturnsLogged()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { email = "admin@admin.com", password = "mypassword" }), Encoding.UTF8, "application/json");
            var responseFromLogin = await client.PutAsync("api/authentication/login", content);
            Assert.Equal(HttpStatusCode.OK, responseFromLogin.StatusCode);
        }

        public class SampleStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                var temp = Path.GetRandomFileName();
                services.AddFolkeCore<SqliteDriver>(options =>
                {
                    options.Elm = elmOptions => elmOptions.ConnectionString = $"Data Source={temp}.db";
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
