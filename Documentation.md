# Caching – Data
Data caching is specifically caching data retrieval operations, such as getting items from a database. Below are different tools that should be leveraged when caching data:

## Automatic Caching using IRepository w/ Attributes
Repository Caching is mostly handled automatically, using AutoFac and intercepts.

### How to Use
1. Create an Interface that inherits `IRepository`, with methods that start with `Get`
1. Create an implementation of that Repository
1. If needed, Decorate methods with `[CacheDependency()]` attributes

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

If you are upgrading from 12.29.0's MVCCaching, you may need to either specify to overwrite the `CachingRepositoryDecorator.cs` or manually merge in the following code into yours:

``` 
		/// <summary>
        /// Returns the cached result for the specified method invocation, if possible. Otherwise proceeds with the invocation and caches the result.
        /// Only results of methods starting with 'Get' are affected.
        /// </summary>
        /// <param name="invocation">Method invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            if (!mCacheEnabled || !invocation.Method.Name.StartsWith("Get", StringComparison.Ordinal))
            {
                invocation.Proceed();

                return;
            }

            var returnType = invocation.Method.ReturnType;

            var cacheDependencyAttributes = invocation.MethodInvocationTarget.GetCustomAttributes<CacheDependencyAttribute>().ToList();
			var doNotCacheAttributes = invocation.MethodInvocationTarget.GetCustomAttributes<DoNotCacheAttribute>().ToList();

            // Either Cache or Retrieve, can modify and include custom logic for DependencyCacheKey generation
			if (doNotCacheAttributes.Count > 0) 
			{
				invocation.Proceed();
			}
			else if (cacheDependencyAttributes.Count > 0)
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyFromAttributes(cacheDependencyAttributes, invocation.Arguments));
            }
            else if (typeof(TreeNode).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType));
            }
            else if (typeof(IEnumerable<TreeNode>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForPage(returnType.GenericTypeArguments[0]));
            }
            else if (typeof(BaseInfo).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType));
            }
            else if (typeof(IEnumerable<BaseInfo>).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = GetCachedResult(invocation, GetDependencyCacheKeyForObject(returnType.GenericTypeArguments[0]));
            }
            else
            {
                invocation.Proceed();
            }
        }
```

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

# Caching Renderings (Output Caching)
The other type of data we wish to focus on is generally called “Output Caching.”  This is caching that is intended to reduce load times or rendering times once the data has been retrieved.  Here are the following caching scenarios, their pros and cons and how to implement.

## Cache Entire Output ('Donut-Hole + Donut')
In this model, the entire output is cached.  This is the default behavior of `[OutputCache]` attribute in MVC, in which the Logic and View rendering are all cached together.  

The positive side of this is it’s very easy to implement, and for very static sites it’s often great to leverage.

The downside though is that (unless you do ajax calls client side) none of the content within the rendering can be dynamic and vary per user without generating unique caches for each variation.  The entire result is cached, so if there is any variation, that’s another entirely cached result for that user.

The other downside is cache dependencies become difficult, as in theory you would need to know not only the main content’s dependencies, but also any dependencies in the header and footer.  Say you add a Navigation Item, you *should* invalidate ALL Output Caches since the main Layout has changed for all of them.

### Implementation
1. Use the `[OutputCache()]` Attribute on your `ActionResult`.

See [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedViewByID`

## Partial View Caching ("Donut-Hole")
In this model, the main `ActionResult` is not cached, but instead individual elements of the rendering are cached.  For example, your Layout view calls a Cached Partial View of the Navigation, and a Cached Partial View of the Footer.  

The Positive side is then each section can have its own Cache Dependencies and can clear individual element’s caches if that cache needs clearing.

The Downside is the Logic done in your `ActionResult` is not cached, so that can increase load times, and any element not cached of course will render fully each time.

### Implementation
1. Do not use the `[OutputCache]` attribute on your main `ActionResult`
1. Add `[OutputCache]` Attributes on any Partial View on elements that you want to cache
1. Call `@Html.RenderAction` or `@Html.RenderView` in your Layouts to call these cached elements.

See [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`IndexByID` and the [ExampleSharedLayout.cshtml](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Views/ExampleCache/ExampleSharedLayout.cshtml) `@{Html.RenderAction("CachedPartialExample", "ExampleCache", new { Index = 1 }); }` of [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedPartialExample`

## Action Result Caching ("Donut" ?)
Action Result Caching is a combination of both worlds.  In this, the Logic of the main `ActionResult` is cached, but the rendering View is not.

The Positive you can still cache individual elements on the page (such as the navigation) and have those on separate Cache Dependencies, and you are caching the logic to render the Model that is passed to the view in your main ActionResult.

The Downside is if you want your Action Result’s View to be cached (the items not found in the Layout), then you may have to call separate Cached Partial Views and pass parts of your Model out, thus segmenting your View.  

Let’s use the example of an ActionResult RenderBlog that gets a Blog Article with Related Blogs.  While the Retrieval of the Model of the Blog + Related Blogs is cached, the Repeater to display all those Related Blogs would not be cached unless you made a separate Partial View (ex `@Html.RenderAction(“RenderRelatedBlogs”, Model.RelatedBlogs)`) and cached that.

### Implementation
1. Use `[ActionResultCache]` on your main `ActionResult`
1. Use `[OutputCache]` on Partial Views that you wish to cache.  
1. Call `@Html.RenderAction` or `@Html.RenderView` in your Layouts to call these cached elements.

[ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedActionByID` and [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController).`CachedPartialExample`.

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

## Examples
Please see the below files for examples:
* [ExampleCacheController.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController.cs) for multiple variations of `[OutputCache]` parameters
* [Global.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs).`GetVaryByCustomString` for how to handle `VaryByCustom`
* [OutputCacheKeyOptionExtension.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Extensions/OutputCacheKeyOptionsExtension.cs) -> How to add `VaryByExtensions`
* [ExampleCacheKey.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Extensions/ExampleCacheKey.cs) -> Example Cache Key implementation

## Setting Cache Dependencies
As with the Caching, Cache dependencies on your output cache are important.  How these operate is Cache Dependencies are added to the Current `HttpResponse`, then cache handlers can use those to find out which responses should be cleared when a cache dependency is touched.

The [CMSRegistrationSource.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Infrastructure/Caching/Implementations/CMSRegistrationSource.cs) implements Kentico’s various services which include it’s Cache Dependency checks and clearing of those caches when the appropriate elements are updated in the database.

To leverage this however, an `IOutputCacheDependency` interface was created to more easily allow the addition of Cache Item Dependencies.

### Implementation
1. On your Class Constructor, add a Private Member of type `IOutputCacheDependencies'
1. Include `IOutputCacheDependencies OutputCacheDependencies` in your constructor, and save that value to your private member.
1. Then, call the private member's `AddCacheItemDependencies(IEnumerable<string>)` or `AddCacheItemDependency(string)`. These will add the dependencies.

### Note for Repositories Returning Generic Models
When calling various Repositories, I recommend you include adding `IEnumerble<string> Get_____CacheDependencies()` methods , so even the cache keys can be abstracted out.  Since a cache dependency key may look different on Kentico EMS vs say Kentico Cloud or some other repository.

See [ExampleCacheController](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico.Examples/Controllers/Examples/ExampleCacheController.cs).`CacheByActionID`
