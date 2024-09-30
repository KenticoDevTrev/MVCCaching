using CMS.ContentEngine;
using MVCCaching;
using System;
using System.Collections.Generic;
using XperienceCommunity.MVCCaching.Enums;

namespace XperienceCommunity.MVCCaching.Extensions.Obsolete
{
    /// <summary>
    /// These are obsolete ones, used for transitioning between KX13 and XbyK, you can include this namespace and to get guidance on where to migrate your dependencies, none of these perform any actions on the ICacheDependencyBuilder and simply return the builder.
    /// </summary>
    public static class ICacheDependencyBuilderObsoleteExtensions
    {
        #region "Obsolete methods"

        [Obsolete("Use ContentType or WebpageType")]
        public static ICacheDependencyBuilder PageTypeDefinition(this ICacheDependencyBuilder builder, string className) => builder;

        [Obsolete("There is no longer a concept of Webpage Order for cache dependencies, please use either the ParentPath with the parent item, or if it's a related Content Item, then on the referencing webpage/content item itself as the order is part of the field value.")]
        public static ICacheDependencyBuilder PageOrder(this ICacheDependencyBuilder builder) => builder;

        [Obsolete("Use either ChannelWebpageType (Channel Specific but only for Webpage items) or ContentType (Channel Agnostic)")]
        public static ICacheDependencyBuilder SitePageType(this ICacheDependencyBuilder builder, string className) => builder;

        [Obsolete("Use WebpagePath")]
        public static ICacheDependencyBuilder PagePath(this ICacheDependencyBuilder builder, string path, PathTypeEnum type = PathTypeEnum.Explicit) => builder;

        [Obsolete("Use ContentItems, ContentItemsOnLanguage, Webpages, or WebpagesOnLanguage")]
        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<IContentItemFieldsSource> pages) => builder;

        [Obsolete("Use WebpageOnLanguage or ContentItemOnLanguage (will need the ContentItemID and ContentItemLanguageMetadataContentLanguageID)")]
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, int? documentId) => builder;

        [Obsolete("Use WebpageOnLanguage or ContentItemOnLanguage (will need the ContentItemID and ContentItemLanguageMetadataContentLanguageID)")]
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, int documentId) => builder;

        [Obsolete("Use ContentItemOnLanguage or WebpageOnLanguage (will need the ContentItemGUID and ContentItemLanguageMetadataContentLanguageID)")]
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, Guid? documentGUID) => builder;

        [Obsolete("Use ContentItem or Webpage (will need the ContentItemGUID or WebPageItemGUID)")]
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, Guid documentGUID) => builder;

        [Obsolete("Use WebpagesOnLanguage or ContentItemsOnLanguage (will need the ContentItemID and ContentItemLanguageMetadataContentLanguageID)")]
        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<int?> documentIds) => builder;

        [Obsolete("Use WebpagesOnLanguage or ContentItemsOnLanguage (will need the ContentItemID and ContentItemLanguageMetadataContentLanguageID)")]
        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<int> documentIds) => builder;

        [Obsolete("Use ContentItem or Webpage (will need the ContentItemID or WebPageItemID)")]
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, int? nodeId) => builder;

        [Obsolete("Use ContentItem or Webpage (will need the ContentItemID or WebPageItemID)")]
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, int nodeId) => builder;

        [Obsolete("Use ContentItem or Webpage (will need the ContentItemGUID or WebPageItemGUID)")]
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, Guid? nodeGUID) => builder;

        [Obsolete("Use ContentItem or Webpage (will need the ContentItemGUID or WebPageItemGUID)")]
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, Guid nodeGUID) => builder;

        [Obsolete("Custom Table Items no longer exist, should be replaced with specific Custom Module Class")]
        public static ICacheDependencyBuilder CustomTableItems(this ICacheDependencyBuilder builder, string codeName) => builder;

        [Obsolete("Custom Table Items no longer exist, should be replaced with specific Custom Module Class")]
        public static ICacheDependencyBuilder CustomTableItem(this ICacheDependencyBuilder builder, string codeName, int? customTableItemId) => builder;

        [Obsolete("Custom Table Items no longer exist, should be replaced with specific Custom Module Class")]
        public static ICacheDependencyBuilder CustomTableItem(this ICacheDependencyBuilder builder, string codeName, int customTableItemId) => builder;

        [Obsolete("Node Relationships no longer exist, instead use the Kentico given ILinkedItemsDependencyRetriever/IWebPageLinkedItemsDependencyRetriever (passing values both to the IProgressiveCache's CacheBuilder as well as passing them out to be added to the CacheDependencyBuilder (you can use DTOWrapperForAdditionalDependencies for this).")]
        public static ICacheDependencyBuilder NodeRelationships(this ICacheDependencyBuilder builder, int? nodeId) => builder;

        [Obsolete("Node Relationships no longer exist, instead use the Kentico given ILinkedItemsDependencyAsyncRetriever/IWebPageLinkedItemsDependencyAsyncRetriever (passing values both to the IProgressiveCache's CacheBuilder as well as passing them out to be added to the CacheDependencyBuilder (you can use DTOWrapperForAdditionalDependencies for this).")]
        public static ICacheDependencyBuilder NodeRelationships(this ICacheDependencyBuilder builder, int nodeId) => builder;

        [Obsolete("Attachments no longer exist, if it's a Media Library item use the Media Extension, or if it has been moved to a specific Media Content Item, use the ContentItem method")]
        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, Guid? attachmentGUID) => builder;

        [Obsolete("Attachments no longer exist, if it's a Media Library item use the Media Extension, or if it has been moved to a specific Media Content Item, use the ContentItem method")]
        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, Guid attachmentGUID) => builder;

        #endregion
    }
}
