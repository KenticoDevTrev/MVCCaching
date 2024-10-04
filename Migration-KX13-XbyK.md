

# Migrating from XperienceCommunity.MVCCaching 13 to XbyK (29.5.0+) (MVCCaching.Kentico[/.Core ]
MVCCaching.Kentico started out back in the Kentico 12 MVC days, and was my first attempt at transitioning from the Portal Engine's way of handling Dependency Keys (often found in Webparts).

As 13 came out, this tool was upgraded.  Dependency Injection still difficult in .Net 4.8, but i attempted to carry over much of that functionality originally in the MVCCaching.Kentico.

With KX13 .Net Core, and new interfaces such as `IProgressiveCache` and `IPageRetriever` available, much of the functionality pieces of the MVCCaching became outdated, and not recommended.

With Xperience by Kentico not much has changed between 13 and XbyK.  The largest changes have to do with the Extension Methods for Dependency Building and some adjustments to how it handles Channels (formerly Sites) and Members (formerly Users)

This is the migration guide to moving from those systems to XperienceCommunity.MVCCaching

# ICacheDependencyBuilder Extension Methods
As Kentico Xperience 13 used "Node" and "Document", where as Xperience by Kentico has Webpage, ContentItem, many of the extension methods have been remade.  This normally won't impact much as you will be needing to use new APIs anyway.  

I have added a `ICacheDependencyBuilderObsoleteExtensions.cs` which has it's own namespace, and contains obsolete warnings and guidance on what to switch to if you have problems.

# New ICachingReferenceService
Not all the pertinant information needed for Dependency Key generation is available by default when retrieving data.  Things like LanguageName and ChannelName often need to be retrieved from LanguageID and ChannelId/WebsiteChannelId.  

This `ICachingReferenceService` can be leveraged to translate these.  The data is often cached as a whole and stored in a dictionary since Languages and Channels are rarely updated.

# Query Extension Split
In Kentico Xperience 13, the DocumentQuery was responsible for retrieving documents, and the extension method `WithCulturePreviewModeContext(_cacheRepositoryContext)` was provided to ensure the language and preview mode was honored.

In Xperience by Kentico, the Language and Preview Modes are split between two objects (the `ContentItemQueryBuilder` and `ContentQueryExecutionOptions`), so separate extension methods have been provided for these.

`contentItemQueryBuilder.WithCultureContext(CacheRepositoryContext)` and `ContentQueryExecutionOptions.WithPreviewModeContext(CacheRepositoryContext)`

# DTOWithDependencies and Linked Objects / In-Cache Dependencies
Xperience by Kentico introduced `Linked` items in the Content Hub.  This is a great way to model data and to break it down into sub-items, and easily link them together.  This also means, however, that as part of data retrieval it often has to dynamically find and retrieve these linked items.  This adds additional dependency key requirements aside from your normal keys.  For example, before you may want to get Blog Items (and the dependency key would be on just the Blog Item type), but now Blog Items may link to Authors, tagging, and other sub-Content Items.

Kentico provides the `ILinkedItemsDependencyAsyncRetriever` or `IWebPageLinkedItemsDependencyAsyncRetriever` which should be leveraged within your cache method (`IProgressiveCache`), but then these dependency keys need to be passed back outside of your cache method so they can be shared with the `ICacheDependencyBuilder` and thus, added to the Cache Scope.

This is where the `DTOWithDependencies<T>` comes in.  This allows you to wrap your returned results (Result) along with a `List<string>` of AdditionalDependencies (like those from Linked Objects), `List<IWebPageFieldSource>` WebPageItems, `List<IContentItemFieldSource>` ContentItems, or `List<BaseInfo>` Objects. 

You can then use the `ICacheDependencyBuilder.AppendDTOWithDependencies` to add these to your builder afterwards.

# ICacheRepositoryContext extension obsoletion
Previously the ICacheRepositoryContext had an extension `.ToCacheRepositoryContextNameIdentifier()`, this is now obsolete, the ICacheRepositoryContext now inherits ICacheKey so the object extension `.ToCacheNameIdentifier()` now properly returns a cache name with preview and language.

# Removal of IContentItemMetadataProvider
This was used to retrieve code names from generic `TreeNode` and `ObjectInfo` classes in Kentico Xperience 13.  This shouldn't be necessarily anymore, and the APIs to retrieve these items no longer exist anyway.