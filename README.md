# MVC Caching
MVC Caching for Kentico MVC, extending the DancingGoat Caching Implementation to allow for easy Caching both of Repository calls, database calls, and Output Caching.  Sets up and leverages Kentico's cache dependencies.

# Installation for Kentico MVC Site (Quick)
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

# Installation for Kentico MVC Site (Manual)
1. Install the `MVCCaching.Base` NuGet Package to your MVC Site
1. Go to the [MVCCaching.Kentico](https://github.com/KenticoDevTrev/MVCCaching/tree/master/MVCCaching.Kentico) Folder on GitHub
1. Copy and include the Infrastructure into your Kentico MVC Site
1. Add the code found in the [Global.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) Application_Start method into your application's Application_Start
1. If your Application's class name in your Global.asax is not "MvcApplication", then replace `MvcApplication` in the `Infrastructure\Caching\Startup\DependencyResolverConfig.cs` file with your class name.
1. Optionally add the [Globa.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) GetVaryByCustomString method.

# Usage
1. See [Documentation.md](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Documentation.md) for full documentation

# Contributions, but fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and i will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Kentico 12 SP site (hotfix 29 or above).
