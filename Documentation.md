# Upgrading to 12.29.2
If you are upgrading from 12.29.0 or 12.29.1's MVCCaching, you may need to either specify to overwrite the `CachingRepositoryDecorator.cs` during the NuGet update or manually merge the changes into yours.  Please see the latest [CachingRepositoryDecorator.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Interceptor/CachingRepositoryDecorator.cs), 12.29.2 made sweeping changes to the entire class.

You also will need to update the [DependencyResolverConfig.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Startup/DependencyResolverConfig.cs) at with this code (passing the `IOutputCacheDependencies` to the `CachingRepositoryDecorator`'s constructor).

``` csharp
// Register caching decorator for repositories
builder.Register(context => new CachingRepositoryDecorator(GetCacheItemDuration(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled(), context.Resolve<IOutputCacheDependencies>()))
    .InstancePerRequest();
```

These changes enable automatic addition of the method's Cache Dependencies into the Request's Output Cache Dependencies.  This way if you leverage MVC OutputCache, it will gain the dependencies that your repositories also have automatically.

Also, In 12.29.0 and 12.29.1, there was a requirement that the method start with `Get` in order to be cached, **this was removed in 2.29.2**

# Caching – Data
Data caching is specifically caching data retrieval operations, such as getting items from a database. Below are different tools that should be leveraged when caching data:

## Automatic Caching using IRepository w/ Attributes
Repository Caching is mostly handled automatically, using AutoFac and intercepts.

### How to Use
1. Create an Interface that inherits `IRepository`
1. Create an implementation of that Repository
1. If needed, Decorate methods with `[CacheDependency()]` attributes

**Automatic Caching will occur under these conditions:**
1. The Method is public.
2. It does NOT contains `[DoNotCache]` and/or it does NOT contains a `[CacheDuration(0)]`
3. It either contains 1 or more `[CacheDependency()]` attributes, or returns `TreeNode`, `IEnumerable<TreeNode>`, `BaseInfo`, or `IEnumerable<BaseInfo>`, or has a `[CacheDuration(#)]` with a number greater than 0
4. The method is called through the `IRepository` inherited Interface.


Autofac is set up to look for any method that starts with Get and implements the `IRepository` interface ([CachingRepositoryDecorator.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Interceptor/CachingRepositoryDecorator.cs)).  It then intercepts (`CachingRepositoryDecorator.Intercept`) these methods and then based on either the `CacheDependency` attributes, or the return type, it creates a Cache Dependency for the method and runs it through Kentico’s CacheHelper (`CachingRepositoryDecorator.GetCachedResult`).

#### Culture and Latest Version Enabled

It is currently configured that any `IRepository` inherited class that has a constructor with the parameters `string cultureName` and/or `bool latestVersionEnabled` will have these values passed based on the [DependencyResolverConfig.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Startup/DependencyResolverConfig.cs)  `ConfigureDependencyResolverForMvcApplication`.  If your repositories need these values, please add them to your constructor so you can leverage them in your API calls.

You can see how to leverage these in the [KenticoExamplePageTypeRepository.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Repositories/Implementations/KenticoExamplePageTypeRepository.cs)

### CacheDependency Attribute

The `CacheDependency` Attribute take a String with the `DependencyKeyFormat`.  You can use `{#}` in this format to pull in the Parameters from the call.  If the Cache Dependencies require dynamic values that are not passed to the method, then you may need to do custom caching inside of the function.

[KenticoExamplePageTypeRepository.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Repositories/Implementations/KenticoExamplePageTypeRepository.cs) for examples.

### PagesCacheDependency Attribute

Similar to the `CacheDependency`, except takes a Page Type and constructs the `node|sitename|pagetype|all` dependency.

### DoNotCache Attribute

The `DoNotCache` Attribute will bypass the automatic caching that occurs through the IRepository logic found in `CachingRepositoryDecorator.cs`  This is useful if you wish to either not cache, or wish to implement your own caching (using ICacheHelper / Kentico's CacheHelper).

### CacheDuration Attribute

The `CacheDuration` Attribute will set the number of minutes an item will be cached.  If set to 0, caching will be disabled. If this is the only attribute (there is no [CacheDependency] nor it return the specified types), and the duration is set to a number greater than 0, then the method will cache with a dummy Dependency Key (random guid).  This means there will be no way to specifically clear this cache except if you clear the entire cache.

### CacheNameNoCulture Attribute

If you add a `[CacheNameNoCulture]` attribute then the Cache Key name will not contain the culture (which exists by default), which will mean that a user requesting this data from a different culture will share the same cache key and cache.   This should be added for methods calls that are site-agnostic.

### CacheNameNoSite Attribute

If you add a `[CacheNameNoSite]` attribute then the Cache Key name will not contain the site name (which exists by default), which means that a user requesting this data from a different site will share the same cache key and cache.  This should be added for methods calls that are culture-agnostic.

### Nuances
* If you apply custom `CacheDependency` attributes, these will be used only instead of the automatic dependency generation.
* Both `CacheDependency` and `PagesCacheDependency` replace  `##SITENAME##` with the Current Site's Code Name in the DependencyKeyFormat thanks to the [CachingRepositoryDecorator.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Interceptor/CachingRepositoryDecorator.cs) `GetDependencyCacheKeyFromAttributes`
* If your method returns any class that is of type `BaseInfo` or `IEnuemrable<BaseInfo>` it applies a cache dependency of `ObjectType|all`.  If it returns an object of class `TreeNode`, or `IEnumerable<TreeNode>`, then it applies a cache dependency of `nodes|[sitename]|[classname]|all.`

## Repository that returns BaseInfo/TreeNode vs. Generic Model

While the Cache Dependency is set up to detect `BaseInfo` and `TreeNode`, this means your Repository Interface and Implementation is returning a Kentico EMS specific object. This does break one of the points of having your Repository’s abstracted into Interface, since you should be able to switch out your implementation with other technologies (say Kentico Cloud).

Having a Generic Model has many possible advantages in that your controllers and views are potentially decoupled from Kentico’s API, meaning Unit Testing is relatively easy, swapping out sources can be easier (such as Kentico Cloud), and upgrades should require much less refactoring (since all Kentico API code is segregated to the Repository and Service Implementations).

But it does come with a potentially significant overhead, such as creating new Generic Models for each content type you are pulling in from Kentico, time to convert those models, and loss of the "automatic" cache dependency logic.

Having your Repositories return `BaseInfo`/`TreeNode` types so the [CachingRepositoryDecorator](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Interceptor/CachingRepositoryDecorator.cs) can add automatic dependencies for these calls, as well as leveraging the `OutputCacheDependencies` helper methods will make working with Cache a lot quicker.

## Adhoc Caching
If you need something specifically cached, feel free to either use the `ICacheHelper` interface (Adding a property of this type to your Class Constructor will automatically provide you with the Kentico Implementation), or you can bypass that and use the `Kentico.Helpers.CacheHelper` where applicable.

## Other Structures
Along with using `IRepository` for your data retrieval, you should practice using IService Interfaces and implementations if any data manipulation or retrieval requires technology specific calls.  For example, if you have a service that retrieves Page Banners for a given page, that should be abstracted out so it too can be easily swapped out.

# Handling Dynamic Cache Dependency Keys
There are situations where your cache dependency keys are more complicated than what can be created through the `[CacheDependency()]` Attribute.  Since the automatic caching and Output Cache Dependencies are only aware of the `[CacheDependency()]` attributes and the method's return type, in these situations it cannot automatically Cache nor add Output Cache Dependencies.

For both examples, we'll use this method as our starting point:

``` csharp
// Sample starting method, has dynamic keys
[DoNotCache]
public IEnumerable<AccordionGroup> GetRelatedAccordionsComplex(Guid nodeGuid, int nodeId, string[] regionFilter = null)
{
    List<string> dependencyKeyList = new List<string>();
    string siteName = SiteContext.CurrentSiteName;
    return CacheHelper.Cache(cs =>
    {
        var query = AccordionGroupProvider.GetAccordionGroups()
            .LatestVersion(_latestVersionEnabled)
            .Published(!_latestVersionEnabled)
            .InAdhocRelationshipWith(nodeId, "Accordions")
            .Culture(_cultureName)
            .OnSite(SiteContext.CurrentSiteName)
            .CombineWithDefaultCulture();

        IEnumerable<AccordionGroup> accordions = query.TypedResult;
        if (accordions != null && accordions.Any())
        {
            dependencyKeyList.Add($"nodeid|{nodeId}|relationships");
            dependencyKeyList.Add($"nodeguid|{siteName}|{nodeGuid}");
            foreach (var accordion in accordions)
            {
                dependencyKeyList.Add($"nodeguid|{siteName}|{accordion.NodeGUID}");
            }
        }
        if (cs.Cached)
        {
            cs.CacheDependency = CacheHelper.GetCacheDependency(dependencyKeyList.ToArray());
        }
        return accordions;
    }, new CacheSettings(CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), "dhp|data|accordionrelationship", siteName, _cultureName, _latestVersionEnabled, nodeGuid, regionFilter));
}
```

**Split up the Methods (Ideal)**
You may abe able to split up your method into multiple smaller methods that *can* be handled through normal caching.  For example, if you are getting a page's related items, you would have cache on both the Page's relationships and the related items found.  Keep in mind though, that the automatic caching happens through the Interface Interceptor, which means you have to call the method *through your Dependency Injection* (using `DependencyResolver.Current.GetService<T>()`), or have these methods in a separate Interface that inherits the `IRepository` interface which you include in the default constructor of your implementation.

So the above method can be split up into this:

``` csharp
// This method is from the interface itself.
[DoNotCache]
public IEnumerable<AccordionGroup> GetRelatedAccordions(Guid nodeGuid, int nodeId)
{
    // This is called through the Dependency Injection and is cached and it's dependencies added to output cache
    IEnumerable<int> AccordionIDs = DependencyResolver.Current.GetService<IAccordionRepository>().GetRelatedAccordionsIDs(nodeGuid, nodeId);
    // This is called through the Dependency Injection and is cached and it's dependencies added to output cache
    IEnumerable<AccordionGroup> Accordions = AccordionIDs.Select(x => DependencyResolver.Current.GetService<IAccordionRepository>().GetAccordion(x));
    return Accordions;
}

[CacheDependency("nodeid|{1}|relationships")]
[CacheDependency("nodeguid|##SITENAME###|{0}")]
public IEnumerable<int> GetRelatedAccordionsIDs(Guid nodeGuid, int nodeId, string[] regionFilter = null)
{
    var query = AccordionGroupProvider.GetAccordionGroups()
        .LatestVersion(_latestVersionEnabled)
        .Published(!_latestVersionEnabled)
        .InAdhocRelationshipWith(nodeId, "Accordions")
        .Culture(_cultureName)
        .OnSite(SiteContext.CurrentSiteName)
        .CombineWithDefaultCulture()
        .Columns("DocumentID");

    return query.TypedResult.Select(x => x.DocumentID);
}

[CacheDependency("documentid|{0}")]
public AccordionGroup GetAccordion(int DocumentID)
{
    return AccordionGroupProvider.GetAccordionGroups()
        .WhereEquals("DocumentID", DocumentID)
        .FirstOrDefault();
}
```

**Returning the Ouput Cache Dependencies (doable but messy)**
The other option is that in your Interface and implementation, you add an `out IEnumerable<string> OutputCacheDependencies` property to the method.  This would be a clear indicator to the coder that they need to handle the output cache dependency keys themselves in their usage of it.  It's not ideal by any stretch.

This is what that would look like:

``` csharp
[DoNotCache]
public IEnumerable<AccordionGroup> GetRelatedAccordions(Guid nodeGuid, int nodeId, out IEnumerable<string> OutputCacheDependencies)
{
    var siteName = SiteContext.CurrentSiteName;
    var AccordionsWithKeys = CacheHelper.Cache(cs =>
    {
        var query = AccordionGroupProvider.GetAccordionGroups()
            .LatestVersion(_latestVersionEnabled)
            .Published(!_latestVersionEnabled)
            .InAdhocRelationshipWith(nodeId, "Accordions")
            .Culture(_cultureName)
            .OnSite(SiteContext.CurrentSiteName)
            .CombineWithDefaultCulture();

        IEnumerable<AccordionGroup> accordions = query.TypedResult;

        var dependencyKeyList = new List<string>();
        if (accordions != null && accordions.Any())
        {
            dependencyKeyList.Add($"nodeid|{nodeId}|relationships");
            dependencyKeyList.Add($"nodeguid|{siteName}|{nodeGuid}");
            foreach (var accordion in accordions)
            {
                dependencyKeyList.Add($"nodeguid|{siteName}|{accordion.NodeGUID}");
            }
        }
        if (cs.Cached)
        {
            cs.CacheDependency = CacheHelper.GetCacheDependency(dependencyKeyList.ToArray());
        }
        // Returning a Tuple so the dependency keys are passed back
        return new Tuple<IEnumerable<AccordionGroup>, string[]>(accordions, dependencyKeyList.ToArray());
    }, new CacheSettings(CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), "dhp|data|accordionrelationship", siteName, _cultureName, _latestVersionEnabled, nodeGuid, regionFilter));
    
    // Set the output cache dependencies so the user can handle them
    OutputCacheDependencies = AccordionsWithKeys.Item2;
    
    // Return the result
    return AccordionsWithKeys.Item1;
}
```
And the usage:
``` csharp
List<string> Dependencies = new List<string>();
// Call from IAccordionRepository
mAccordionRepository.GetRelatedAccordions(NodeGuid, NodeID, out Dependencies);
// use IOutputCacheRepository to add them to the output cache
mOutputCacheDependencies.AddCacheDependencies(Dependencies.ToArray());
```

# Caching Renderings (Output Caching)
The other type of data we wish to focus on is generally called “Output Caching.”  This is caching that is intended to reduce load times or rendering times once the data has been retrieved.  Here are the following caching scenarios, their pros and cons and how to implement.  First though, let's cover some definitions of the Output Caching:

## Output Cache Variables
Of the `[OutputCache]` and `[ActionResultCache]`, there are a handful of properties that I wish to explain:

### Duration
Time in seconds the cache should be enacted

### VaryByParam
MVC Defaults to all parameters, but you can specify in a semi-colon separated list which parameters should be included in making the CacheKey.  `none` is the keyword to signify you wish to ignore the Parameters altogether. 

### VaryByCustom
This acts as a sort of keyword that you can add multiple, even dynamic values to your CacheKey.  How it works is when you add a value, the Application’s `GetVeryByCustomString(HttpContext context, string custom)` is called and returns the CacheKey to be added.  You can overwrite this method in your [Global.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) to add different options. 

Examples are provided of how to convert this custom string into the cache key, and how to make your own `VaryBy` extension methods.  Kentico provides some defaults (VaryByHost, VaryByBrowser, VaryByUser, VaryByPersona, VaryByCookieLevel, VaryByABTestVariant), but you [can create your own](https://github.com/KenticoDevTrev/MVCCaching/tree/master/MVCCaching.Kentico.Examples/Extensions).  In the end, all these do is concatenate all the Variations into a long string for the CacheKey (Ex `User=Trevor_Browser=Chrome`)

### VaryByHeader
You can specify Header Parameters that should be included in the Cache Key creation.  Can be useful if an API can return XML or JSON depending on the header’s content-type value, you would want to cache the XML and JSON separately.

### VaryByCookie
You can specify Cookie values that should be included in the Cache Key Creation.

## Output Cache Dependency Automation
As a request is rendered, it gathers Output Cache Dependencies along the way.  As part of the MVCCaching tool, any `IRepository` typed methods that are called through their interface will automatically add the same Cache Dependencies to the Output Cache Dependencies.

You can also manually add dependencies through the `IOutputCacheDependency` interface, which you can add to the constructor of your classes and leverage.

## Cache Model: Cache Entire Output ('Donut-Hole + Donut')
In this model, the entire output is cached.  This is the default behavior of `[OutputCache]` attribute in MVC, in which the Logic and View rendering are all cached together.  

The positive side of this is it’s very easy to implement, and is the 'fastest' cache as the entire output is cached.  

The downside though is that (unless you do ajax calls client side) none of the content within the rendering can be dynamic and vary per user without generating unique caches for each variation.  The entire result is cached, so if there is any variation, that’s another entirely cached result for that user.

The other downside is you need to make sure that all the elements have their dependencies added to the output cache, as in theory you would need to know not only the main content's dependencies, but also any dependencies in the header and footer.  Say you add a Navigation Item, you *should* invalidate ALL Output Caches since the main Layout has changed for all of them.  It is important to ensure that you include all the possible cache dependencies for the rendering either through usage of automatic `IRepository` caching or manually through the `IOutputCacheDependency` interface.

### Implementation
1. Use the `[OutputCache()]` Attribute on your `ActionResult`.

See [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedViewByID`

## Cache Model: Partial View Caching ("Donut-Hole")
In this model, the main `ActionResult` is not cached, but instead individual elements of the rendering are cached.  For example, your Layout view calls a Cached Partial View of the Navigation, and a Cached Partial View of the Footer.  

The Positive side is then each section can have its own Cache Dependencies and can clear individual element’s caches if that cache needs clearing.

The Downside is the Logic done in your `ActionResult` is not cached, so that can increase load times, and any element not cached of course will render fully each time.

### Implementation
1. Do not use the `[OutputCache]` attribute on your main `ActionResult`
1. Add `[OutputCache]` Attributes on any Partial View on elements that you want to cache
1. Call `@Html.RenderAction` or `@Html.RenderView` in your Layouts to call these cached elements.

See [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`IndexByID` and the [ExampleSharedLayout.cshtml](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Views/ExampleCache/ExampleSharedLayout.cshtml) `@{Html.RenderAction("CachedPartialExample", "ExampleCache", new { Index = 1 }); }` of [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedPartialExample`

## Cache Model: Action Result Caching ("Donut" ?)
Action Result Caching is a combination of both worlds.  In this, the Logic of the main `ActionResult` is cached, but the rendering View is not.

The Positive you can still cache individual elements on the page (such as the navigation) and have those on separate Cache Dependencies, and you are caching the logic to render the Model that is passed to the view in your main ActionResult.

The Downside is if you want your Action Result’s View to be cached (the items not found in the Layout), then you may have to call separate Cached Partial Views and pass parts of your Model out, thus segmenting your View.  

Let’s use the example of an ActionResult RenderBlog that gets a Blog Article with Related Blogs.  While the Retrieval of the Model of the Blog + Related Blogs is cached, the Repeater to display all those Related Blogs would not be cached unless you made a separate Partial View (ex `@Html.RenderAction(“RenderRelatedBlogs”, Model.RelatedBlogs)`) and cached that.

### Implementation
1. Use `[ActionResultCache]` on your main `ActionResult`
1. Use `[OutputCache]` on Partial Views that you wish to cache.  
1. Call `@Html.RenderAction` or `@Html.RenderView` in your Layouts to call these cached elements.

[ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedActionByID` and [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedPartialExample`.

## Examples
Please see the below files for examples:
* [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController.cs) for multiple variations of `[OutputCache]` parameters
* [Global.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs).`GetVaryByCustomString` for how to handle `VaryByCustom`
* [OutputCacheKeyOptionExtension.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Extensions/OutputCacheKeyOptionsExtension.cs) -> How to add `VaryByExtensions`
* [ExampleCacheKey.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Extensions/ExampleCacheKey.cs) -> Example Cache Key implementation

## Setting Output Cache Dependencies
As with the Data Caching, Output Cache Dependencies are important.  How these operate is Cache Dependencies are added to the Current `HttpResponse`, then cache handlers can use those to find out which responses should be cleared when a cache dependency is touched.

The [CMSRegistrationSource.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Implementations/CMSRegistrationSource.cs) implements Kentico’s various services which include it’s Cache Dependency checks and clearing of those caches when the appropriate elements are updated in the database.

To leverage this however, an `IOutputCacheDependency` interface was created to more easily allow the addition of Cache Item Dependencies.

### Implementation
1. On your Class Constructor, add a Private Member of type `IOutputCacheDependencies'
1. Include `IOutputCacheDependencies OutputCacheDependencies` in your constructor, and save that value to your private member.
1. Then, call the private member's `AddCacheItemDependencies(IEnumerable<string>)` or `AddCacheItemDependency(string)`. These will add the dependencies.
