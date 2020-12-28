# MVC Caching
MVC Caching for Kentico MVC, extending the DancingGoat Caching Implementation to allow for easy Caching both of Repository calls, database calls, and Output Caching (MVC Full Framework only).  Sets up and leverages Kentico's cache dependencies.

# Kentico 13 MVC.Net Core Installation
1. Install the Nuget Package `MVCCaching.Kentico.Core` on your MVC.Net Core application.  Note due to the complexity of invocation decoration, this package does depend on Castle Windsor and Autofac.

2. On your Main method (usually Program.cs), to the HostBuilder, add .UserServiceProviderFactory(new AutofacServiceProviderFactory())
example: 
```csharp
public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                  // THIS ONE LINE BELOW NEEDED:
                 .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
```
3. On your Startup Class Constructor method, add the following:
```csharp
public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
            
            // MVC Caching
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
            // END MVC Caching
        }
```
4. Add these 2 methods to your Startup Class:
```csharp
public IConfigurationRoot Configuration { get; private set; }
public ILifetimeScope AutofacContainer { get; private set; }
```
5. Make sure for your Startup.ConfigureServices you have `services.AddOptions();` called
6. Add the following method to your Startup Class
```csharp
// MVC Caching
public void ConfigureContainer(ContainerBuilder builder)
{
        DependencyResolverConfig.Register(builder, new Assembly[] { typeof(Startup).Assembly });
}
```

These steps hook up Autofac and MVC Caching.

**NOTE**: The "string currentCulture" and "bool previewEnabled" have been replaced with `IRepoContext` which you can leverage to retrieve these values.

# Kentico 13 MVC.Net Framework Installation

Installation instructions to come (once i package for .net framework)

# Kentico 12 Note on CastleCore and AutoFac
Some have reported issues, expecially when upgrading from 12.29.2 to a higher version, and some with a "fresh" install, that they receive an error about not finding a version of AutoFac that is compatible with Autofac.Extras.DynamicProxy2.  If your Autofac is not version 3.5.2, then you will need to revert Autofac to 3.5.2 once installed, also make sure Autofac.Extras.DynamicProxy is uninstalled if you have that vs. Autofac.Extras.DynamicProxy2

# Installation for Kentico 12 MVC Site (Quick)
1. Install the `MVCCaching.Kentico` NuGet Package to your MVC Site
1. Add this to your Global.asax.cs's Application_Start 
```
        #region "AutoFac Cache and other dependency injections"

        // Register AutoFac Items
        var builder = new ContainerBuilder();

        // Register Dependencies for Cache
        DependencyResolverConfig.Register(builder);

        // Set Autofac Dependency resolver to the builder
        DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));

        #endregion
```
1. If your Application's class name in your Global.asax is not "MvcApplication", then replace `MvcApplication` in the `Infrastructure\Caching\Startup\DependencyResolverConfig.cs` file with your class name.
1. Optionally create an override the GetVaryByCustomString method to your Application (see the [sample Globa.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) GetVaryByCustomString method).

# Usage
1. See [Documentation.md](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Documentation.md) for full documentation

# Kentico 12 Possible Castle.Core issue upon Upgrading
If you upgrade and receive an error on your MVC site similar to `Could not load file or assembly Castle.Core, Version=4.0.0.0` or `Could not load file or assembly Castle.Core, Version=4.1.0.0`

Go to your Web.config and look for the Castle.Core dependentAssembly tag, replace with this.  It's confusing but although 4.2.0.0 is installed, the binding redirect needs to go to 4.0.0.0
 ```
 <dependentAssembly>
    <assemblyIdentity name="Castle.Core" publicKeyToken="407dd0808d44fbdc" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-999.999.999.999" newVersion="4.0.0.0" />
  </dependentAssembly>
  ```

# Contributions, bug fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and i will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Kentico 12 SP site (hotfix 29 or above), and Kentico 13
