


# XperienceCommunity.MVCCaching
This package contains interfaces and extensions to build, store, and use Cache Dependencies, as well as an Attribute to automatically set up Dependency Injection on your custom interfaces/implementations.

This version is only compatible Kentico Xperience 13 with .Net Core (3.1+) applications.  If you are running KX12 MVC or KX13 on MVC 5 (.Net 4.8 Framework), please see the [.Net 4.8 branch](https://github.com/KenticoDevTrev/MVCCaching/tree/MVCCaching_For_Net48) and packages.

# Migration
If migrating from KX12 MVCCaching.Kentico, or KX13 MVCCaching.Kentico / MVCCaching.Kentico.Core, please see the [Migration Readme](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Migration.md)

# Installation

1. Install the Nuget Package `XperienceCommunity.MVCCaching` on your MVC.Net Core application.  
  - If you have Kentico-Agnostic libraries and need to implement the basic interfaces (such as `ICacheKey`) or leverage the Automatic DI, you can add the `MVCCaching.Base.Core` nuget package.

3. On your IServiceCollection, add MVCCaching and optionally the automatic Dependency injection: 
```csharp
public void ConfigureServices(IServiceCollection services)
        {
        services.AddMVCCaching();
        // optional Automatic DI setup, see documentation
	    // services.AddMVCCachingAutoDependencyInjectionByAttribute();
	    // services.AddMVCCachingAutoDependencyInjectionBySuffixes(new string[] {"Repository", "Service"});
	    ...
        }
```


# Usage

## Caching in Xperience / MVC
Kentico Xperience provides it's integrated Cache Dependency system, which triggers cache clearing when certain objects are touched, and it's own `IProgressiveCache` and `IPageRetriever` interfaces which have caching built in.

During the calling of these operations, you define Cache Dependency Keys, which are attached to that cached operation . If the keys get touched (can be manually touched or automatically by Kentico Xperience), the caches automatically 'clear' for that item.

MVC.Net Core provides output caching using the `<cache>` tag helper.  Kentico Xperience has added a `<cache-depencency />` tag helper within that to allow you to pass cache dependencies, and thus also properly clear the `<cache>` content if the keys are touched.

**The problems are**:
1. There is no default way to know what all cache dependencies are added within a `<cache>` tag
2.  Any logic / dependencies defined within `IProgressiveCache` / `IPageRetriever` are only executed if the cache misses.

## How MVCCaching Works
The MVCCaching System works by solving the 2 problems mentioned above, as well as adding useful tools.

First, it provides the `ICacheDependencyStore` and `ICacheDependencyScope` interfaces.  

The `ICacheDependencyStore` stores cache dependencies in a central scoped array, which makes it possible to determine what dependencies are called within the `<cache>` tag. 

The `ICacheDependencyScope` allows you to tracking dependencies between the `Begin()` and `End()`. Simply call `ICacheDependencyScope.Begin()` before you make any operations that may have caching/cache dependencies, then call `ICacheDependencyScope.End()` into the `<cache-dependency key=@scope.End() />`

**SAMPLE**
```html
@inject ICacheDependencyScope CacheScope 
<cache expires-after=@CacheMinuteType.Long.ToTimeSpan() >
	@{
		CacheScope.Begin();
	}
	<vc:some-thing />
    <cache-dependency cache-keys="@CacheScope.End()" />
</cache>
```

**SAMPLE View Component**
```csharp
public async Task<IViewComponentResult> InvokeAsync()
        {
            _cacheDependenciesScope.Begin();
            // This repository should leverage the ICacheDependencyStore via ICacheDependencyBuilderFactory/ICacheDependencyBuilder
			var model = await	_someRepsitory.GetStuffAsync();
			return View("mycomponent.cshtml", model);
		}
```
```html 
/// mycomponent.cshtml
@inject ICacheDependencyScope CacheScope 
<cache duration=@CacheMinuteType.Long>
	<h1>@Model.Greeting</h1>
	<p>Some lengthy operation warranty caching...</p>
    <cache-dependency cache-keys="@CacheScope.End()" />
</cache>
```

## ICacheDependencyBuilderFactory and ICacheDependencyBuilder
As mentioned, another issue with dependency keys are that Kentico Xperience's `IPageRetriever` and `IProgressiveCache` need the dependency keys to properly cache, but if you define them within those interfaces, they are only processed if the cache misses and the logic is executed.

Thus, you need to define your cache dependencies **outside** of these interfaces (so the `ICacheDependencyStore` can track them), as well as pass them **into** the Kentico Xperience Interfaces so data-level caching can occur.

MVCCaching introduces the `ICacheDependencyBuilderFactory` interface which has a `Create(bool addKeysToStore = true);` method.  Inject this into your repositories and call this method to retrieve an `ICacheDependencyBuilder` class.

This class has a plethora of built in extension methods to accommodate easily define your dependency keys and can easily be extended.

Additionally, it provides a quick `IPageCacheBuilder.Configure` method to integrate with the `IPageRetriever` interface, and a `ICacheDependencyBuilder.GetCMSCacheDependency()` method to integrate with `IProgressiveCache`

**SAMPLE**
```csharp
// IPageRetriever
public async Task<IEnumerable<TabItem>> GetTabsAsync(string path)
        {
            var builder = _cacheDependencyBuilderFactory.Create()
                .PagePath(path, PathTypeEnum.Children);

            var retriever = await _pageRetriever.RetrieveAsync<Tab>(
                query => query
                    .Path(path, PathTypeEnum.Children)
                    .Columns(new string[] {
                        nameof(Tab.DocumentID),
                        nameof(Tab.TabName)
                    })
                    .OrderBy(nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder)),
                cacheSettings => cacheSettings.Configure(builder, CacheMinuteTypes.Medium.ToDouble(), "GetTabsAsync", path)
            );

            return retriever.Select(x => _mapper.Map<TabItem>(x));
        }

// IProgressiveCache
public async Task<Maybe<RoleItem>> GetRoleAsync(string roleName, string siteName)
        {
            var builder = _cacheDependencyBuilderFactory.Create()
                                .Object(RoleInfo.OBJECT_TYPE, roleName);

            var role = await _progressiveCache.LoadAsync(async cs =>
            {
                if (cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                return await _roleInfoProvider.GetAsync(roleName, await _siteRepository.GetSiteIDAsync(siteName));
            }, new CacheSettings(CacheMinuteTypes.Medium.ToDouble(), "GetRoleAsync", roleName, siteName));

            if (role != null)
            {
                return Maybe.From(_mapper.Map<RoleItem>(role));
            }
            else
            {
                return Maybe.None;
            }
        }

```

## Extending ICacheDependencyBuilder
You can easily create your own extension methods to suit your purposes for your site.  You can reference the [ICacheDependencyBuilderExtensions.cs](#) File in this repository to get an idea of how to add your own. 

If needed as well, you implement your own Factory and Builder object to add even further functionality, however in most cases this is not warranted.

## Querying Pages on other Sites
The `ICacheDependencyBuilderFactory` by default uses the current request's site for it's context.  If you are querying pages from another site, or with a differnet culture than the current culture, then you cannot leverage the default `IPageRetriever` as well as the dependency keys will need to be different.

You can use the `ICacheDependencyBuidlerFactory.Create("MySiteCodeName")` method to overwrite the SiteCodeName that will be used in the cache key building.

Then you should use the normal `DocumentQuery`, `DocumentQuery<TType>` or `MultiDocumentQuery` to retrieve pages.  Unlike the `IPageRetriever` which automatically handles Culture, Site, and PreviewMode, these base queries do not do so.  You can leverage the `query.WithCulturePreviewModeContext(_cacheRepositoryContext)` to add in Culture and Preview Mode, then the `query.Site("MySiteCodeName")` to select your specific site, and in the cache key name use `_cacheRepositoryContext.ToCacheRepositoryContextNameIdentifier()` to add the culture and preview mode to the cache key name, along with your site name.

Here's an example:

``` csharp
private ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory;
private ICacheRepositoryContext _cacheRepositoryContext;
private IProgressiveCache _progressiveCache;

public MyClass(ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
		ICacheRepositoryContext cacheRepositoryContext,
		IProgressiveCache progressiveCache) {
	_cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
	_cacheRepositoryContext = cacheRepositoryContext;
	_progressiveCache = progressiveCache;
}

public Task<IEnumerable<TreeNode>> GetPagesOnDifferentSiteAsync() {
	var siteName = "SomeOtherSiteCodeName";

	// Create builder with specified site
	var builder = _cacheDependencyBuilderFactory.Create(siteName)
		.PagePath("/Some/Path", PathTypeEnum.Children);
		
	var data = _progressiveCache.LoadAsync(async cs => 
		var query = await new DocumentQuery()
			.Site(siteName)
			.WithCulturePreviewModeContext(_cacheRepositoryContext)
			.Path("/Some/Path/%")
			.GetEnumerableTypedResultAsync();
		return query;
		}, new CacheSettings(60, "GetPagesOnDifferentSite", siteName, _cacheRepositoryContext.ToCacheRepositoryContextNameIdentifier()));
		
		return data;
}
```

# Other Tools
## Cache Durations
This package comes with 2 Enum Extension methods, `Enum.ToDouble()` and `Enum.ToTimeSpan()`, this converts the `int` value of the enum into a double or timespan (as minutes).  We recommend creating a Cache Duration Enum (ex `CacheMinutesType` ) that has int values corresponding to the minutes you wish to cache for.  This makes changing and managing different 'durations' of caching easy.

## Object.ToCacheNameIdentifier() / ICacheKey
When building out a Cache Name (unique identifier for the cache to hit on), it must ultimately resolve to a string.  If you are using a model, the .ToString() will not be unique to what your object actually is.  You can implement `ICacheKey` on your model and define the `GetCacheKey()` to return a unique string based on the model itself.

The `object.ToCacheNameIdentifier()` extension method also properly retrieves the object's identifier, be it string.Empty if null, the `ICacheKey.GetCacheKey()` if it implements, or the `object.ToString()` otherwise.  It also handles IEnumerables of objects and joins their own `ToCacheNameIdentifier()` together in a pipe delimited string.

The `IPageCacheBuilder.Configure` extension method automatically leverages this, however if you use `IProgressiveCache`'s `new CacheSettings(duration, nameparams)` you will need to call `.ToCacheNameIdentifier()` on any parameter passed into it if you wish to leverage this functionality.

## ICacheRepositoryContext
This interface provides quick helper methods to determine if the site is in PreviewMode or not and the current culture.  `IProgressiveCache` already handles Preview Mode or not, however your `<cache enabled=bool />` may benefit from the `repoContext.CacheEnabled()` method so it doesn't MVCCache during Preview mode.

`CacheEnabled()` is short for `!PreviewEnabled()`

## IContentItemMetadataProvider
This interface helps retrieve Kentico Xperience's `CodeName` for the given object (`TreeNode` or `BaseInfo`. 

# Contributions, bug fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and I will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Kentico Xperience 13 for .Net Core
