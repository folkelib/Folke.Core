using Folke.Core.Entities;
using Folke.CsTsService;
using Folke.Elm;
using Folke.Identity.Elm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Folke.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFolkeCore(
            this IApplicationBuilder app, 
            IFolkeConnection connection, 
            IHostingEnvironment env,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            ApplicationPartManager applicationPartManager,
            Action<FolkeCoreOptions> optionsAction)
        {
            app.UseIdentity();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseRequestLocalization();

            connection.UpdateIdentityUserSchema<int, User>();
            connection.UpdateIdentityRoleSchema<int, User>();
            connection.UpdateSchema(typeof(User).GetTypeInfo().Assembly);
        
            using (var transaction = connection.BeginTransaction())
            {
                var options = new FolkeCoreOptions();
                optionsAction(options);
                CreateAdministrator(roleManager, userManager, options).GetAwaiter().GetResult();
                transaction.Commit();
            }

            if (env.IsDevelopment())
            {
                CreateTypeScriptServices(applicationPartManager);
            }

            return app;
        }

        private static async Task CreateAdministrator(RoleManager<Role> roleManager, UserManager<User> userManager, FolkeCoreOptions options)
        {
            var administrateur = await roleManager.FindByNameAsync(RoleNames.Administrator);
            if (administrateur == null)
            {
                await roleManager.CreateAsync(new Role { Name = RoleNames.Administrator });
            }

            var users = await userManager.GetUsersInRoleAsync(RoleNames.Administrator);
            if (users.Count == 0)
            {
                var result = await userManager.CreateAsync(new User { UserName = options.AdministratorEmail, Email = options.AdministratorEmail },
                        options.AdministratorPassword);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(options.AdministratorEmail);
                    await userManager.AddToRoleAsync(user, RoleNames.Administrator);
                }
            }
        }

        private static void CreateTypeScriptServices(ApplicationPartManager applicationPartManager)
        {
            ControllerFeature feature = new ControllerFeature();
            applicationPartManager.PopulateFeature(feature);
            var controllerTypes = feature.Controllers.Select(c => c.AsType());
            var converter = new Converter();
            var assembly = converter.ReadAssemblies(controllerTypes);
            var typeScript = new TypeScriptWriter();
            typeScript.WriteAssembly(assembly);
            typeScript.WriteToFiles("src/services");
        }
    }
}
