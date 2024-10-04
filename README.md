


# XperienceCommunity.DevTools.MVCCaching
This package contains interfaces and extensions to build, store, and use Cache Dependencies, as well as an Attribute to automatically set up Dependency Injection on your custom interfaces/implementations.

This version is only compatible Xperience by Kentico with .Net 8+ applications.  If you are running KX13 on .Net Core, see the [README_KX13.md](README_KX13.md), or if you're using KX12 MVC or KX13 on MVC 5 (.Net 4.8 Framework), please see the [.Net 4.8 branch](https://github.com/KenticoDevTrev/MVCCaching/tree/MVCCaching_For_Net48) and packages.

# Migration
If migrating from KX13 XperienceCommunity.MVCCaching, please see the [Migration Readme](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Migration-KX13-XbyK.md)

If migrating from KX12 MVCCaching.Kentico, you'll want to combine the instructions in the [Migration KX12MVC to KX13 Readme](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Migration-KX12MVC-to-KX13.md) and the [Migration KX13 to XbyK Readme](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Migration-KX13-XbyK.md)

# Installation

1. Install the Nuget Package `XperienceCommunity.DevTools.MVCCaching` on your MVC.Net Core application.  
  - If you have Kentico-Agnostic libraries and need to implement the basic interfaces (such as `ICacheKey`) or leverage the Automatic DI, you can add the `MVCCaching.Base.Core` nuget package, and for any razor-enabled project, `MVCCaching.Base.Core.Components`.

3. On your IServiceCollection, add MVCCaching and optionally the automatic Dependency injection: 
```csharp
public void ConfigureServices(IServiceCollection services)
        {
        services.AddMVCCaching();
        // optional Automatic DI setup, see documentation
	    // services.AddMVCCachingAutoDependencyInjectionByAttribute(); // This looks for any class with [AutoDependencyInjection] and injects it for any interface it implements
	    // services.AddMVCCachingAutoDependencyInjectionBySuffixes(new string[] {"Repository", "Service"}); // This looks at any class with these suffixes and injects it for any interface it implements
	    ...
        }
```

Optionally add the Tag Helper for the MVCCaching.Base.Core.Components tag helpers, this adds to the `<cache>` tag the attributes `vary-by-preview` and `scoped`
```html
@addTagHelper *, MVCCaching.Base.Core.Components
```

# Usage

## Caching in Xperience / MVC
Xperience by Kentico provides it's integrated Cache Dependency system, which triggers cache clearing when certain objects are touched, and it's own `IProgressiveCache` interface which have caching built in.

During the calling of these operations, you define Cache Dependency Keys, which are attached to that cached operation . If the keys get touched (can be manually touched or automatically by Xperience by Kentico), the caches automatically 'clear' for that item.

ASP.NET Core MVC provides output caching using the `<cache>` tag helper.  Xperience by Kentico has added a `<cache-dependency />` (or MVCCaching's `<cache-dependency-mvc />`) tag helper within that to allow you to pass cache dependencies, and thus also properly clear the `<cache>` content if the keys are touched.

**The problems are**:
1. There is no default way to know what all cache dependencies are added within a `<cache>` tag
2.  Any logic / dependencies defined within `IProgressiveCache` is only executed if the cache misses.

## How MVCCaching Works
The MVCCaching System works by solving the 2 problems mentioned above, as well as adding useful tools.

First, it provides the `ICacheDependencyStore` and `ICacheDependencyScope` interfaces.  

The `ICacheDependencyStore` stores cache dependencies in a central scoped array, which makes it possible to determine what dependencies are called within the `<cache>` tag. 

The `ICacheDependencyScope` allows you to tracking dependencies between the `Begin()` and `End()`. Simply call `ICacheDependencyScope.Begin()` before you make any operations that may have caching/cache dependencies, then call `ICacheDependencyScope.End()` into the `<cache-dependency key=@scope.End() />` or `<cache-dependency-mvc key=@scope.End() />`

**SAMPLE**
```html
@inject ICacheDependencyScope CacheScope 
<cache expires-after=@CacheMinuteType.Long.ToTimeSpan() >
	@{
		CacheScope.Begin();
	}
	<vc:blogs x-path="/tech" />
    <cache-dependency cache-keys="@CacheScope.End()" />
</cache>
```

**SAMPLE View Component**
```csharp
public async Task<IViewComponentResult> InvokeAsync(string xPath)
        {
            _cacheDependenciesScope.Begin();
            // This repository should leverage the ICacheDependencyStore via ICacheDependencyBuilderFactory/ICacheDependencyBuilder
			var model = await	_blogRepository.GetBlogs(xPath);
			return View("mycomponent.cshtml", model);
		}
```
```html 
/// mycomponent.cshtml
@inject ICacheDependencyScope CacheScope 
<cache duration=@CacheMinuteType.Long>
    @foreach(var blog in Model.Blogs) {
	    <h1>@blog.BlogTitle</h1>
	    <p>Some lengthy operation warranty caching...</p>
    }
    <cache-dependency cache-keys="@CacheScope.End()" />
    @* OR <cache-dependency-mvc cache-keys="@CacheScope.End()"> *@
</cache>
```

## ICacheDependencyBuilderFactory and ICacheDependencyBuilder
As mentioned, another issue with dependency keys are that Xperience by Kentico's `IProgressiveCache` need the dependency keys to properly cache, but if you define them within those interfaces, they are only processed if the cache misses and the logic is executed.

Thus, you need to define your cache dependencies **outside** of this interfaces (so the `ICacheDependencyStore` can track them), as well as pass them **into** the Xperience by Kentico `IProgressiveCache` so data-level caching can occur.

MVCCaching introduces the `ICacheDependencyBuilderFactory` interface which has a `Create(bool addKeysToStore = true);` method.  Inject this into your repositories and call this method to retrieve an `ICacheDependencyBuilder` class.

This class has a plethora of built in extension methods to accommodate easily define your dependency keys and can easily be extended.

Additionally, it provides a quick `ICacheDependencyBuilder.GetCMSCacheDependency()`

**SAMPLE**
```csharp
// IPageRetriever
public Task<IEnumerable<BlogItem>> GetBlogs(string path)
{
    // Create the Cache Dependency Builder and set the dependencies
    var builder = CacheDependencyBuilderFactory.Create()
        .WebPagePath(path, PathTypeEnum.Children);

    var results = ProgressiveCache.LoadAsync(async cs => {

        var nestedLevel = 2;

        // Build Query with Linked Items
        var queryBuilder = new ContentItemQueryBuilder()
            .WithCultureContext(CacheRepositoryContext) // Applys Language Context;
            // Main item we are calling
            .ForContentType("Sample.Blog", subqueryParameters => 
                subqueryParameters.ForWebsite(
                    websiteChannelName: WebsiteChannelContext.WebsiteChannelName,
                    pathMatch: PathMatch.Children(path),
                    includeUrlPath: true
                    )
                    .Columns(nameof(Blog.BlogTitle), nameof(Blog.BlogSummary))
                // Include linked items to 2 nested levels, which will grab our Author and Address
                .WithLinkedItems(nestedLevel)
            )
            // Good practice to apply columns even to sub items
            .ForContentType("Sample.Author", subqueryParameters => 
                subqueryParameters.Columns(nameof(Author.AuthorName))
            )
            .ForContentType("Sample.AuthorAddress", subqueryParameters => 
                subqueryParameters.Columns(nameof(AuthorAddress.AuthorAddressState), nameof(AuthorAddress.AuthorAddressCity))
            );


        // Apply Preview Mode Context
        var queryOptions = new ContentQueryExecutionOptions()
            .WithPreviewModeContext(CacheRepositoryContext);

        // Get Items
        var items = await ContentQueryExecutor.GetMappedResult<Blog>(queryBuilder, queryOptions);

        // Get Linked Item Dependencies
        var linkedDependencies = await LinkedItemsDependencyAsyncRetriever.Get(items.Select(x => x.ContentItemID), nestedLevel);

        // Apply cache dependencies, including the additional linked dependencies
        if(cs.Cached) {
            cs.CacheDependency = builder.GetCMSCacheDependency(additionalDependencies: linkedDependencies);
        }

        // Optional convert items into a lightweight DTO to be returned
        var blogItems = items.Select(x => x.ToBlogItemDTO()); // ToBlogItemDTO is just an extension method that converts the Xperience Webpage Blog to the generic BlogItem DTO

        // Return the results with a wrapper
        return new DTOWithDependencies<IEnumerable<BlogItem>>(blogItems, linkedDependencies);
    }, new CacheSettings(60, "GetBlogs", path, CacheRepositoryContext.ToCacheNameIdentifier()));

    // Append added dependencies
    builder.AppendDTOWithDependencies(results);

    // Return actual results
    return results.Result;
}
```

## Caching for Preview Mode and Cache Enabled
As you cache your data, you must decide if you want the data to cache during Preview mode or not.  

As a general rule of thumb, if it's something a user is going to be editing and needs to see the changes, then you wouldn't want to cache it.

However, if it's an expensive operation and isn't something heavily edited, it may increase the Editor's experience on other pages if widely used views and data calls remain cached even in preview mode.

When you want caching disable for Preview Mode, you must do the following:

**Data Caching for Preview**
When using the `IProgressiveCache`, you have two options when taking Preview into consideration.

You can disable cache on preview by the `cs` (CacheSettings) `Cached` value to the `ICacheRepositoryContext.CacheEnabled()` value (false for preview).  This will prevent any caching if it's in preview mode.

Or, you can elect to simply have a separate cache for Preview by using the `CacheRepositoryContext.ToCacheNameIdentifier()` which includes both the current language and if it's in preview mode or not.  This will product a cached version for both Preview and Non Preview, which is what is recommended.

```cs
    return await _progressiveCache.LoadAsync(async cs =>
            {
                // Disable cache on preview so editors can see this, optional if you want to completely disbale caching on preview
                cs.Cached = _cacheRepositoryContext.CacheEnabled();

                if(cs.Cached)
                {
                    cs.CacheDependency = builder.GetCMSCacheDependency();
                }
                ...
            }, new CacheSettings(CacheMinuteTypes.VeryLong.ToDouble(), "SomeUniqueName", otherKeys
            // This adds the Preview mode and Language to the Cache repository name
            CacheRepositoryContext.ToCacheNameIdentifier()));
```

This will disable the cache all together.

**Output Caching**
When using the `<cache>` tags, you should either use the `scoped` attribute, or set `enabled=@CacheRepositoryContext.CacheEnabled()` within it if you wish to disable caching on Preview.

```html
@addTagHelper *, MVCCaching.Base.Core.Components

// Enabled only if preview mode is false
<cache enabled=@CacheRepositoryContext.CacheEnabled() ... >

</cache>

/* Enabled only if preview mode is false, 
    also sets default duration to the CMS settings key for cache minutes IF you don't have a expires-after set,
    and ensuers user and contact dependency keys are added if vary-by-user and vary-by-contact are added
    */
<cache scoped ... >

</cache>

// Always enabled, same cache for preview and live
<cache enabled=true>

</cache>

// Always enabled, one cache for preview and one for live
<cache enabled=true vary-by-preview >

</cache>
```

Note that, for some reason, if you do NOT set the `enabled` parameter, it seems to default to `false` when in development mode, and true when in production/release, resulting in possible testing issues.  Using the `scoped` attribute will set this value always.  If you always want it to `true`, you should explicitly set it to true.


## Extending ICacheDependencyBuilder
You can easily create your own extension methods to suit your purposes for your site.  You can reference the [ICacheDependencyBuilderExtensions.cs](#) File in this repository to get an idea of how to add your own. 

If needed as well, you implement your own Factory and Builder object to add even further functionality, however in most cases this is not warranted.

## Querying Content Items on different Channels / Languages
The `ICacheDependencyBuilderFactory` by default uses the current request's channel for any dependency key that requires the channel name.  If you are querying items from another channel, you can use the `ICacheDependencyBuidlerFactory.Create("MyChannelCodeName")` method to overwrite the SiteCodeName that will be used in the cache key building.

Then you should use the normal `ContentItemQueryBuilder`, `ContentQueryExecutionOptions` to define channel, language, etc.

Here's an example:

```csharp
// IPageRetriever
public Task<IEnumerable<BlogItem>> GetBlogs(string path)
{
    var channelName = "BlogChannel";
    // Create the Cache Dependency Builder and set the dependencies
    var builder = CacheDependencyBuilderFactory.Create(channelName)
        .WebPagePath(path, PathTypeEnum.Children);

    var results = ProgressiveCache.LoadAsync(async cs => {

        var nestedLevel = 2;

        // Build Query with Linked Items
        var queryBuilder = new ContentItemQueryBuilder()
            .WithCultureContext(CacheRepositoryContext) // Applys Language Context;
            // Main item we are calling
            .ForContentType("Sample.Blog", subqueryParameters => 
                subqueryParameters.ForWebsite(
                    websiteChannelName: channelName,
                    pathMatch: PathMatch.Children(path),
                    includeUrlPath: true
                    )
                    .Columns(nameof(Blog.BlogTitle), nameof(Blog.BlogSummary))
                // Include linked items to 2 nested levels, which will grab our Author and Address
                .WithLinkedItems(nestedLevel)
            )
            // Good practice to apply columns even to sub items
            .ForContentType("Sample.Author", subqueryParameters => 
                subqueryParameters.Columns(nameof(Author.AuthorName))
            )
            .ForContentType("Sample.AuthorAddress", subqueryParameters => 
                subqueryParameters.Columns(nameof(AuthorAddress.AuthorAddressState), nameof(AuthorAddress.AuthorAddressCity))
            );


        // Apply Preview Mode Context
        var queryOptions = new ContentQueryExecutionOptions()
            .WithPreviewModeContext(CacheRepositoryContext);

        // Get Items
        var items = await ContentQueryExecutor.GetMappedResult<Blog>(queryBuilder, queryOptions);

        // Get Linked Item Dependencies
        var linkedDependencies = await LinkedItemsDependencyAsyncRetriever.Get(items.Select(x => x.ContentItemID), nestedLevel);

        // Apply cache dependencies, including the additional linked dependencies
        if(cs.Cached) {
            cs.CacheDependency = builder.GetCMSCacheDependency(additionalDependencies: linkedDependencies);
        }

        // Optional convert items into a lightweight DTO to be returned
        var blogItems = items.Select(x => x.ToBlogItemDTO()); // ToBlogItemDTO is just an extension method that converts the Xperience Webpage Blog to the generic BlogItem DTO

        // Return the results with a wrapper
        return new DTOWithDependencies<IEnumerable<BlogItem>>(blogItems, linkedDependencies);
    }, new CacheSettings(60, "GetBlogs", path, CacheRepositoryContext.ToCacheNameIdentifier()));

    // Append added dependencies
    builder.AppendDTOWithDependencies(results);

    // Return actual results
    return results.Result;
}
```

## Cache Scope Tag Helper

The tag helper exends the [CacheTagHelper](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/cache-tag-helper?view=aspnetcore-5.0) with the Enabled and ExpireAfter attributes set to a default if not passed.

The Enabled attribute will default to the Preview state of the application, if preview mode is enabled the cache attribute will be disabled.

If the ExpiresAfter attribute is not passed it will default to the Xperience System setting for Server Side Caching found in Settings -> System -> Performance -> Cache content (minutes):

### Example
```cs
public class AlertsViewComponent : ViewComponent
{
    private readonly IPageRetriever mPageRetriever;
    private readonly ICacheDependencyBuilderFactory mCacheDependencyBuilderFactory;
    
    public AlertsViewComponent(IPageRetriever pageRetriever, ICacheDependencyBuilderFactory cacheDependencyBuilderFactory)
    {
        mPageRetriever = pageRetriever;
        mCacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var builder = mCacheDependencyBuilderFactory.Create()
                        .PagePath("/Alerts", PathTypeEnum.Children);

        var result = await mPageRetriever.RetrieveAsync<Alert>(
                query => query
                    .Path("/Alerts", PathTypeEnum.Children)
                    .OrderBy(nameof(TreeNode.NodeLevel), nameof(TreeNode.NodeOrder)),
                cacheSettings => cacheSettings.Configure(builder, 60, "GetTabsAsync", "/Alerts")
        );

        var model = result.Select(AlertViewModel.ToViewModel);

        return View("~/Components/ViewComponents/Alerts/Default.cshtml", model);
    }
}
```

```html 
<!-- Will add scoped cache dependency keys -->
<cache scoped>
    <vc:alerts  />
</cache>
<!-- Will add scoped cache dependency keys along with the additional keys passed -->
<cache scoped additional-keys="@(new [] { $"{Alert.CLASS_NAME}|all" })">
    <vc:alerts />
</cache>
<!-- Will add scoped cache dependency keys along with the additional keys passed and override the expiresafter value from the system settings -->
<cache scoped additional-keys="@(new [] { $"{Alert.CLASS_NAME}|all" })" expires-after="@TimeSpan.FromMinutes(60)">
    <vc:alerts />
</cache>
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
This interface helps retrieve Xperience by Kentico's `CodeName` for the given object (`TreeNode` or `BaseInfo`. 

# Contributions, bug fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and I will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Xperience by Kentico 13 for .Net Core
