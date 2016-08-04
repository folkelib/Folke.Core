Getting Started
===============

project.json
^^^^^^^^^^^^^

Create an new empty ASP.NET Core project using your favorite tool (yeoman, Visual Studio, etc.). In the project.json file, add a reference to ``Folke.Core`` and ``Folke.Elm.MySql``:

::

 {
  "version": "1.0",
  "title": "Folke",
  "description": "Folke Homepage",
  "dependencies": {
   "Microsoft.NETCore.App": {
   "version": "1.0.0",
   "type": "platform"
   },
   "Microsoft.AspNetCore.Diagnostics": "1.0.0",
   "Microsoft.AspNetCore.Server.Kestrel": "1.0.0",
   "Microsoft.Extensions.Logging.Console": "1.0.0",
   "Folke.Core": "1.3.0.1",
   "Folke.Elm.Mysql": "1.3.0"
  },
  .....

.. note:: Note that if you prefer to use another database driver, Elm also supports PostgreSQL, Microsoft SQL Server and SQLite.
.. note:: Elm is the ORM developped by the Folke team.

Startup.cs
^^^^^^^^^^

Now in the  ``Startup.cs`` file we need to register the various Folke services. This is done in one line in the ``ConfigureServices`` method:

.. code-block:: c#

 public void ConfigureServices(IServiceCollection services)
 {
    services.AddFolkeCore<MySqlDriver>(options =>
    {
        options.ConnectionString = Configuration["Data:ConnectionString"];
    });
 }

The ``MySqlDriver`` requires that you use the ``Folke.Elm.MySql`` namespace. If you need to use another kind of database you must reference the appropriate driver. The ``ConnectionString`` option is a simple string. In this example it comes from an ``IConfigurationRoot`` property defined earlier in the code:

.. code-block:: c#

 public IConfigurationRoot Configuration { get; }

 public Startup(IHostingEnvironment env)
 {
    var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.Local.json", optional: true)
        .AddEnvironmentVariables();
    Configuration = builder.Build();
 }

For reference, this is what the appsettings.json file looks like:

.. code-block:: javascript

 {
    "Logging": {
        "IncludeScopes": false,
        "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Information"
        }
    },
    "Data": {
        "ConnectionString": "Server=localhost;Database=someDb;Uid=somUser;Pwd=somePassword;",
        "DefaultAdministratorUserName": "admin@folke.co",
        "DefaultAdministratorPassword": "ThisShouldBeChanged!"
    }
 }

We now need to add Folke to the request pipeline. For this we need to modify the ``Configure`` method and add all the types that the framework needs:

.. code-block:: c#

 public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IFolkeConnection connection,
            RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationPartManager applicationPartManager)
        {
            loggerFactory.AddConsole();
            app.UseMvc();

            app.UseFolkeCore(connection, env, roleManager, userManager, applicationPartManager, options =>
            {
                options.AdministratorEmail = Configuration["Data:DefaultAdministratorUserName"];
                options.AdministratorPassword = Configuration["Data:DefaultAdministratorPassword"];
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

The part of this code that is relevant to Folke is the ``app.UseFolkeCore()`` block. The section below describes the required parameters.

.. code-block:: c#

 IApplicationBuilder.UseFolkeCore(IFolkeConnection connection, IHostingEnvironment env, RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationPartManager applicationPartManager)

IFolkeConnection
````````````````

This object holds the database connection parameters. This is a compoment of Folke.Elm, the ORM the framework uses.

IHostingEnvironment
```````````````````

This object holds the hosting environment options (path, development or production environment, etc).

RoleManager<Role>
``````````````````

This class is used to manage roles like modifying one or creating a new one. The ``Role`` class inherits from  ``IdentityRole`` of Folke.Identity.Elm which is an implementation of Microsoft.AspNet.Identity developped to work with Folke.Elm.

UserManager<User>
``````````````````

This class is used to manage users like modifying one or creating a new one. The ``User`` class inherits from ``IdentityUser`` of Folke.Identity.Elm which is an implementation of Microsoft.AspNet.Identity developped to work with Folke.Elm.

ApplicationPartManager
``````````````````````

This class is used to easily access the Folke.Identity.Server controllers (such as AuthenticationController) directly from the framework.