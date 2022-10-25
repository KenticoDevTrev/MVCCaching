

# Migrating from MVCCaching.Kentico[/.Core ]
MVCCaching.Kentico started out back in the Kentico 12 MVC days, and was my first attempt at transitioning from the Portal Engine's way of handling Dependency Keys (often found in Webparts).

As 13 came out, this took was upgraded.  Dependency Injection still difficult in .Net 4.8, but i attempted to carry over much of that functionality originally in the MVCCaching.Kentico.

With KX13 .Net Core, and new interfaces such as `IProgressiveCache` and `IPageRetriever` available, much of the functionality pieces of the MVCCaching became outdated, and not recommended.

This is the migration guide to moving from those systems to XperienceCommunity.MVCCaching

# Dependency Injection
The previous renditions required AutoFac / Castle Windsor to handle dependency injection.  this required an [elaborate installation processes](https://github.com/KenticoDevTrev/MVCCaching/tree/MVCCaching_For_Net48).    You can remove the following nuget packages that are no longer needed:

- [Autofac.Extensions.DependencyInjection](https://www.nuget.org/packages/Autofac.Extensions.DependencyInjection/)  (>= 7.1.0)
-   [Autofac.Extras.DynamicProxy](https://www.nuget.org/packages/Autofac.Extras.DynamicProxy/)  (>= 6.0.0)
-   [Castle.Core](https://www.nuget.org/packages/Castle.Core/)  (>= 4.4.1)

Likewise, you can remove all the steps found in the [original installation processes](https://github.com/KenticoDevTrev/MVCCaching/tree/MVCCaching_For_Net48) if you currently are using it.

The new setup (found on the readme.md) involves just adding the base nuget packages and calling 1-2 methods on your `IServiceCollection`

## Scoped in Singleton Usage
By default, you aren't allowed to use a Transient Lifespan class in a Scoped/Singleton, nor can you use a Scoped Lifespan class in a Singleton.  Autofac automatically handled this for you, so you may run into errors when Autofac is removed.

To overcome this, you can can do one of two things:

1. Inject the `IHttpContextAccessor`  and use the `HttpContext.RequestServices.GetRequiredService<IMyService>()`

```csharp
	
        var _scopedService = _httpContextAccessor.HttpContext.ServiceProvider.GetService<IMyScopedService>();
		// Do stuff here
    
```

2. Inject the `IServiceScopeFactory` in your Singleton instance, then in the method itself that you need to use a scoped service, use the below (**NOTE** this will create a *new* scope, which may not be desirable)

```csharp
	using (var scope = _serviceScopeFactory.CreateScope())
    {
        var _scopedService = scope.ServiceProvider.GetService<IMyScopedService>();
		// Do stuff here
    }
```

# IRepository/IService
These interfaces were removed, as using interfaces as functional tags is not recommended.  Instead, they have been replaced with two options:

1. Add  `[AutoDependencyInjection()]` attribute tag that you can place on your `implementing class` and register using `services.AddMVCCachingAutoDependencyInjectionByAttribute()`
- this also allows you to control the scope of your injections, which the prior always injected them as Scoped
2.  Make sure all your implementing classes have a Suffix naming convention, then register them using `services.AddMVCCachingAutoDependencyInjectionBySuffixes(IEnumerable<string> suffixes)`.  You can pass things like "Repository" or "Service" and any class that has an interface that ends with those will be registered.

# Cache____ Attributes Removal
The various `[Cache_____]` attributes that decorated methods have been removed.  This was a stop-gap since at the time Kentico didn't have `IPageRetriever` and `IProgressiveCache` available with easy to use dependency key passing / generating.

Remove these attributes, and instead leverage `IPageRetriever` and/or `IProgressiveCache` along with the `ICacheDependencyBuilderFactory` interface to create a new `ICacheDependencyBuilder` instance, add your dependency keys on it, then pass it to the `IPageRetriever`/`IProgressiveCache`

# MVC 5 (net 4.8 only) - Output/Rendering caching
There is no longer `ActionResultCache` or output caching, so instead leverage the `<cache></cache>` tag helpers, along with the `ICacheDependencyScope` and the `<cache-dependency keys=@cacheScope.End()/>` tag helper to cache the rendering