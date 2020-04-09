# MVC Caching
MVC Caching for Kentico MVC, extending the DancingGoat Caching Implementation to allow for easy Caching both of Repository calls, database calls, and Output Caching.  Sets up and leverages Kentico's cache dependencies.

# Note on upgrading from 12.29.2
In version 12.29.2, there was a dependency on Autofac 4.9.2 to accomodate the added package Autofac.Extras.DynamicProxy 4.3.0. This caused assembly binding issues in Castle.Core. I've reverted to Autofac 3.5.2 and Autofac.Extras.DynamicProxy2 3.0.7 which is more supported by Kentico MVC.

If you are upgrading from 12.29.2, please first revert Autofac to 3.5.2, and uninstall Autofac.Extras.DynamicPRoxy after updating.

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

# Possible Castle.Core issue upon Upgrading
If you upgrade and receive an error on your MVC site similar to `Could not load file or assembly Castle.Core, Version=4.0.0.0` or `Could not load file or assembly Castle.Core, Version=4.1.0.0`

Go to your Web.config and look for the Castle.Core dependentAssembly tag, replace with this.  It's confusing but although 4.2.0.0 is installed, the binding redirect needs to go to 4.0.0.0
 ```
 <dependentAssembly>
    <assemblyIdentity name="Castle.Core" publicKeyToken="407dd0808d44fbdc" culture="neutral" />
    <bindingRedirect oldVersion="0.0.0.0-999.999.999.999" newVersion="4.0.0.0" />
  </dependentAssembly>
  ```

# Contributions, but fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and i will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Kentico 12 SP site (hotfix 29 or above).
