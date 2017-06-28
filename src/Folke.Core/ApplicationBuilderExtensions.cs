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
            Action<FolkeCoreApplicationOptions> optionsAction)
        {
            app.UseIdentity();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseRequestLocalization();

            using (var transaction = connection.BeginTransaction())
            {
                connection.UpdateIdentityUserSchema<int, User>();
                connection.UpdateIdentityRoleSchema<int, User>();
                connection.UpdateSchema(typeof(User).GetTypeInfo().Assembly);

                var options = new FolkeCoreApplicationOptions();
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

        private static async Task CreateAdministrator(RoleManager<Role> roleManager, UserManager<User> userManager, FolkeCoreApplicationOptions applicationOptions)
        {
            var administrateur = await roleManager.FindByNameAsync(RoleNames.Administrator);
            if (administrateur == null)
            {
                await roleManager.CreateAsync(new Role { Name = RoleNames.Administrator });
            }

            var users = await userManager.GetUsersInRoleAsync(RoleNames.Administrator);
            if (users.Count == 0)
            {
                var result = await userManager.CreateAsync(new User { UserName = applicationOptions.AdministratorEmail, Email = applicationOptions.AdministratorEmail },
                        applicationOptions.AdministratorPassword);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(applicationOptions.AdministratorEmail);
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
            var assembly = converter.ReadControllers(controllerTypes);
            var typeScript = new TypeScriptWriter();
            // Call WriteAssembly twice ; once for TypeScript objects and once for Knockout mappings
            typeScript.WriteAssembly(assembly, true);
            typeScript.WriteAssembly(assembly, false);
            typeScript.WriteToFiles("src/services");
        }
    }
}
