# MVCCaching Base and Components
This readme is specifically for the Platform Agnostic MVCCaching packages.  If you are using this for Xperience by Kentico, or Kentico Xperience 13, please see those specific readmes.

## Concept
I believe caching should be a common optimization practice.  Caching, at it's core, consists of these core elements:

1. What to cache (the object)
2. What is that cached call's unique identifier (it's Name)
3. How long should something be cached (it's Duration)
4. What triggers should clear the cache prematurely (it's Dependencies)

Along with that, there may be things that caches often need to know:

1. Language
2. If caching should be disabled (IE Preview mode)

Lastly, in terms of usage of the `<cache>` tag within ASP.Net Core MVC, you also often need:

1. Cache Dependencies for the `<cache>` tag
2. Cache Scope (to determine the dependencies called within the scope that should pass to the cache tag's dependencies)
3. Cache Store (a way to store dependencies as they are generated)

## The Tool
`MVCCaching.Base.Core` Provides interfaces, extension methods and helpers to accomplish these concepts.  These do not, however, actually implement them, as that varies.

`MVCCaching.Base.Core.Components` also provides Tag Helpers that extend the `<cache>` tag, and add the `<cache-dependency-keys>` child tag to allow passing of those dependencies.

Additionally, although not caching related, Dependency Injection automation is also something often employed and is included.

## Extending
These packages have implementations in Kentico Xperience 13 and Xperience by Kentico, however there is nothing preventing implementations to be written for other platforms, or just MVC itself (using memory cache for example).  

If you wish to use these base items and write other implementations, feel free to fork and make a pull request!