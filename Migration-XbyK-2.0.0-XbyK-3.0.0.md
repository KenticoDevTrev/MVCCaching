

# Migrating from XperienceCommunity.DevTools.MVCCaching 2.0.0 to XperienceCommunity.DevTools.MVCCaching 3.0.0 (MVCCaching.Kentico[/.Core ]
When Xperience came out, they implemented their own `ICacheDependencyBuilderFactory`, which made things very, very confusing.  What made it worse is it had internal workings and mechanisms which made extending it very hard.

With the update in Xperience 31.0.0, it is now impossible to create Kentico's `CMS.Helper.CacheDependencyBuilder`, so I needed to pivot.

# ICacheDependencyScopedBuilderFactory
I've renamed the old `ICacheDependencyBuilderFactory` to `ICacheDependencyScopedBuilderFactory`.  Likewise this now returns an `ICacheDependencyScopedBuilder`.  These act as wrappers to Xperience's Cache Dependency Tools.

The `ICacheDependencyScopedBuilder` contains a `CMS.Helper.CacheDependencyBuilder XperienceBuilder {get;}` property within it, granting full access to the actual builder.  I then use Xperience's CacheDependencyBuilder as the source of the keys, only retrieving them to get the values that are used in the scoping.

To Migrate, simply use `ICacheDependencyScopedBuilderFactory.Create()` instead of `MVCCaching.ICacheDependencyBuilderFactory.Create()` and it's `XperienceBuilder` property to build out your keys.  If you don't need scoping, use Xperience's normal `ICacheDependencyBuilderFactory.Create()`.