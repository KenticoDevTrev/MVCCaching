using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCCaching
{
    public static class ICacheDependencyBuilderExtensions
    {

        #region "All based on type"

        public static ICacheDependencyBuilder ContentType(this ICacheDependencyBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"contentitem|byname|{className}");
        }

        public static ICacheDependencyBuilder ContentTypeOnLanguage(this ICacheDependencyBuilder builder, string className, string language)
        {
            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(language)) {
                return builder;
            }

            return builder.AddKey($"contentitem|bycontenttype|{className}|{language}");
        }

        public static ICacheDependencyBuilder WebPageType(this ICacheDependencyBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"webpageitem|byname|{className}");
        }

        public static ICacheDependencyBuilder WebPageTypeOnChannel(this ICacheDependencyBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"webpageitem|bychannel|{builder.SiteName()}|bycontenttype|{className}");
        }

        public static ICacheDependencyBuilder WebPageTypeOnChannel(this ICacheDependencyBuilder builder, string className, string channelName)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"webpageitem|bychannel|{channelName}|bycontenttype|{className}");
        }

        public static ICacheDependencyBuilder WebPageTypeOnChannelAndLanguage(this ICacheDependencyBuilder builder, string className, string language)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"webpageitem|bychannel|{builder.SiteName()}|bycontenttype|{className}|{language}");
        }

        public static ICacheDependencyBuilder WebPageTypeOnChannelAndLanguage(this ICacheDependencyBuilder builder, string className, string channelName, string language)
        {
            if (string.IsNullOrWhiteSpace(className)) {
                return builder;
            }

            return builder.AddKey($"webpageitem|bychannel|{channelName}|bycontenttype|{className}|{language}");
        }

        public static ICacheDependencyBuilder ObjectType(this ICacheDependencyBuilder builder, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) {
                return builder;
            }

            return builder.AddKey($"{typeName}|all");
        }

        #endregion


        #region "Content Items"

        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, IContentItemFieldsSource page)
        {
            if (page.SystemFields.ContentItemID > 0) {
                return builder.ContentItems([page.SystemFields.ContentItemID]);
            } else if (page.SystemFields.ContentItemGUID != Guid.Empty) {
                return builder.ContentItems([page.SystemFields.ContentItemGUID]);
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.ContentItemName)) {
                return builder.ContentItems([page.SystemFields.ContentItemName]);
            }

            throw new CacheDependencyMissingDataException("No ContentItemID, ContentItemGUID, or ContentItemName found, cannot cache, please correct query to include at least one of these.");
        }
        public static ICacheDependencyBuilder ContentItems(this ICacheDependencyBuilder builder, IEnumerable<IContentItemFieldsSource> pages)
        {
            foreach (var page in pages) {
                builder.ContentItem(page);
            }
            return builder;
        }

        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, int contentItemID) => ContentItems(builder, [contentItemID]);
        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, int? contentItemID) => contentItemID.HasValue ? ContentItems(builder, [contentItemID.Value]) : builder;
        public static ICacheDependencyBuilder ContentItems(this ICacheDependencyBuilder builder, IEnumerable<int> contentItemIDs)
        {
            return builder.AddKeys(contentItemIDs.Select(contentItemID => $"contentitem|byid|{contentItemID}"));
        }

        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, Guid contentItemGuid) => ContentItems(builder, [contentItemGuid]);
        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, Guid? contentItemGuid) => contentItemGuid.HasValue ? ContentItems(builder, [contentItemGuid.Value]) : builder;
        public static ICacheDependencyBuilder ContentItems(this ICacheDependencyBuilder builder, IEnumerable<Guid> contentItemGuids)
        {
            return builder.AddKeys(contentItemGuids.Select(contentItemGuid => $"contentitem|byguid|{contentItemGuid}"));
        }

        public static ICacheDependencyBuilder ContentItem(this ICacheDependencyBuilder builder, string contentItemName) => !string.IsNullOrWhiteSpace(contentItemName) ? ContentItems(builder, [contentItemName]) : builder;
        public static ICacheDependencyBuilder ContentItems(this ICacheDependencyBuilder builder, IEnumerable<string> contentItemNames)
        {
            return builder.AddKeys(contentItemNames.Select(contentItemName => $"contentitem|byname|{contentItemName}"));
        }

        #endregion

        #region "Language Specific Content Items"

        public static ICacheDependencyBuilder ContentItemOnLanguage(this ICacheDependencyBuilder builder, IContentItemFieldsSource page, string language)
        {
            if (page.SystemFields.ContentItemID > 0) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemID, language);
            } else if (page.SystemFields.ContentItemGUID != Guid.Empty) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemGUID, language);
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.ContentItemName)) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemName, language);
            }
            throw new CacheDependencyMissingDataException("No ContentItemID, ContentItemGUID, or ContentItemName found, cannot cache, please correct query to include at least one of these.");
        }
        public static ICacheDependencyBuilder ContentItemsOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<IContentItemFieldsSource> pages, string language)
        {
            foreach (var page in pages) {
                builder.ContentItemOnLanguage(page, language);
            }
            return builder;
        }

        public static ICacheDependencyBuilder ContentItemOnLanguage(this ICacheDependencyBuilder builder, IContentItemFieldsSource page, ICacheReferenceService cachingLanguageService)
        {
            var language = cachingLanguageService.GetLanguageNameById(page.SystemFields.ContentItemCommonDataContentLanguageID);
            if (page.SystemFields.ContentItemID > 0) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemID, language);
            } else if (page.SystemFields.ContentItemGUID != Guid.Empty) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemGUID, language);
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.ContentItemName)) {
                return builder.ContentItemOnLanguage(page.SystemFields.ContentItemName, language);
            }
            throw new CacheDependencyMissingDataException("No ContentItemID, ContentItemGUID, or ContentItemName found, cannot cache, please correct query to include at least one of these.");
        }
        public static ICacheDependencyBuilder ContentItemsOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<IContentItemFieldsSource> pages, ICacheReferenceService cachingLanguageService)
        {
            foreach (var page in pages) {
                builder.ContentItemOnLanguage(page, cachingLanguageService);
            }
            return builder;
        }

        public static ICacheDependencyBuilder ContentItemOnLanguage(this ICacheDependencyBuilder builder, int contentItemID, string language) => ContentItemsOnLanguage(builder, [contentItemID], language);
        public static ICacheDependencyBuilder ContentItemsOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<int> contentItemIDs, string language)
        {
            return builder.AddKeys(contentItemIDs.Select(contentItemID => $"contentitem|byid|{contentItemID}|{language}"));
        }

        public static ICacheDependencyBuilder ContentItemOnLanguage(this ICacheDependencyBuilder builder, Guid contentItemGuid, string language) => ContentItemsOnLanguage(builder, [contentItemGuid], language);
        public static ICacheDependencyBuilder ContentItemsOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<Guid> contentItemGuids, string language)
        {
            return builder.AddKeys(contentItemGuids.Select(contentItemGuid => $"contentitem|byguid|{contentItemGuid}|{language}"));
        }

        public static ICacheDependencyBuilder ContentItemOnLanguage(this ICacheDependencyBuilder builder, string contentItemName, string language) => ContentItemsOnLanguage(builder, [contentItemName], language);
        public static ICacheDependencyBuilder ContentItemsOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<string> contentItemNames, string language)
        {
            return builder.AddKeys(contentItemNames.Select(contentItemName => $"contentitem|byname|{contentItemName}|{language}"));
        }

        #endregion

        #region "WebPage Items"

        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, IWebPageFieldsSource page)
        {
            if (page.SystemFields.WebPageItemID > 0) {
                return builder.WebPages([page.SystemFields.WebPageItemID]);
            } else if (page.SystemFields.WebPageItemGUID != Guid.Empty) {
                return builder.WebPages([page.SystemFields.WebPageItemGUID]);
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.WebPageItemName)) {
                return builder.WebPages([page.SystemFields.WebPageItemName]);
            }

            throw new CacheDependencyMissingDataException("No ContentItemID, ContentItemGUID, or ContentItemName found, cannot cache, please correct query to include at least one of these.");

        }
        public static ICacheDependencyBuilder WebPages(this ICacheDependencyBuilder builder, IEnumerable<IWebPageFieldsSource> pages)
        {
            foreach (var page in pages) {
                builder.WebPage(page);
            }
            return builder;
        }

        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, int webPageItemID) => WebPages(builder, [webPageItemID]);
        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, int? webPageItemID) => webPageItemID.HasValue ? WebPages(builder, [webPageItemID.Value]) : builder;
        public static ICacheDependencyBuilder WebPages(this ICacheDependencyBuilder builder, IEnumerable<int> webPageItemIDs)
        {
            return builder.AddKeys(webPageItemIDs.Select(webPageItemID => $"webpageitem|byid|{webPageItemID}"));
        }

        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, Guid webPageItemGuid) => WebPages(builder, [webPageItemGuid]);
        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, Guid? webPageItemGuid) => webPageItemGuid.HasValue ? WebPages(builder, [webPageItemGuid.Value]) : builder;
        public static ICacheDependencyBuilder WebPages(this ICacheDependencyBuilder builder, IEnumerable<Guid> webPageItemGuids)
        {
            return builder.AddKeys(webPageItemGuids.Select(webPageItemGuid => $"webpageitem|byguid|{webPageItemGuid}"));
        }

        public static ICacheDependencyBuilder WebPage(this ICacheDependencyBuilder builder, string webPageName) => !string.IsNullOrWhiteSpace(webPageName) ? WebPages(builder, [webPageName]) : builder;
        public static ICacheDependencyBuilder WebPages(this ICacheDependencyBuilder builder, IEnumerable<string> webPageNames)
        {
            return builder.AddKeys(webPageNames.Select(webPageName => $"webpageitem|byname|{webPageName}"));
        }

        #endregion

        #region "WebPageItems by path"


        public static ICacheDependencyBuilder WebPagePath(this ICacheDependencyBuilder builder, string path, PathTypeEnum type = PathTypeEnum.Explicit) => WebPagePath(builder, path, builder.SiteName(), type: type);

        public static ICacheDependencyBuilder WebPagePath(this ICacheDependencyBuilder builder, IWebPageFieldsSource webpage, ICacheReferenceService cachingReferenceService, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(webpage.SystemFields.WebPageItemTreePath)) {
                throw new CacheDependencyMissingDataException("WebPageItemTreePath is empty, cannot cache, please correct query to include both of this field.");
            }
            return builder.WebPagePath(webpage.SystemFields.WebPageItemTreePath, cachingReferenceService.GetChannelNameByWebsiteChannelId(webpage.SystemFields.WebPageItemWebsiteChannelId), type: type);

        }

        public static ICacheDependencyBuilder WebPagePath(this ICacheDependencyBuilder builder, string path, string channelName, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(path)) {
                return builder;
            }
            var simplePath = path.TrimEnd('%').TrimEnd('/');
            switch (type) {
                case PathTypeEnum.Single:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}");
                    break;
                case PathTypeEnum.Children:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}");
                    break;
                case PathTypeEnum.Section:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}");
                    builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}");
                    break;
                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWith("/%")) {
                        builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}");
                    } else {
                        builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}");
                    }
                    break;
            }

            return builder;
        }

        public static ICacheDependencyBuilder WebPagePathOnLanguage(this ICacheDependencyBuilder builder, string path, string language, PathTypeEnum type = PathTypeEnum.Explicit) => WebPagePathOnLanguage(builder, path, builder.SiteName(), language, type: type);

        public static ICacheDependencyBuilder WebPagePathOnLanguage(this ICacheDependencyBuilder builder, IWebPageFieldsSource webpage, ICacheReferenceService cachingReferenceService, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(webpage.SystemFields.WebPageItemTreePath)) {
                throw new CacheDependencyMissingDataException("WebPageItemTreePath is empty, cannot cache, please correct query to include both of this field.");
            }
            return builder.WebPagePathOnLanguage(webpage.SystemFields.WebPageItemTreePath, cachingReferenceService.GetChannelNameByWebsiteChannelId(webpage.SystemFields.WebPageItemWebsiteChannelId), cachingReferenceService.GetLanguageNameById(webpage.SystemFields.ContentItemCommonDataContentLanguageID), type: type);

        }

        public static ICacheDependencyBuilder WebPagePathOnLanguage(this ICacheDependencyBuilder builder, string path, string channelName, string language, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(path)) {
                return builder;
            }
            var simplePath = path.TrimEnd('%').TrimEnd('/');
            switch (type) {
                case PathTypeEnum.Single:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}|{language}");
                    break;
                case PathTypeEnum.Children:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}|{language}");
                    break;
                case PathTypeEnum.Section:
                    builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}|{language}");
                    builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}|{language}");
                    break;
                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWith("/%")) {
                        builder.AddKey($"webpageitem|bychannel|{channelName}|childrenofpath|{simplePath}|{language}");
                    } else {
                        builder.AddKey($"webpageitem|bychannel|{channelName}|bypath|{simplePath}|{language}");
                    }
                    break;
            }

            return builder;
        }

        #endregion

        #region "Language Specific WebPage Items"

        public static ICacheDependencyBuilder WebPagesOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<IWebPageFieldsSource> pages, string language)
        {
            foreach (var page in pages) {
                builder.WebPageOnLanguage(page, language);
            }
            return builder;
        }
        public static ICacheDependencyBuilder WebPageOnLanguage(this ICacheDependencyBuilder builder, IWebPageFieldsSource page, string language)
        {
            if (page.SystemFields.WebPageItemID > 0) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemID], language);
            } else if (page.SystemFields.WebPageItemGUID != Guid.Empty) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemGUID], language);
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.WebPageItemName)) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemName], language);
            }

            throw new CacheDependencyMissingDataException("No WebPageItemID, WebPageItemGUID, or WebPageItemName found, cannot cache, please correct query to include at least one of these.");
        }

        public static ICacheDependencyBuilder WebPagesOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<IWebPageFieldsSource> pages, ICacheReferenceService cachingLanguageService)
        {
            foreach (var page in pages) {
                builder.WebPageOnLanguage(page, cachingLanguageService);
            }
            return builder;
        }
        public static ICacheDependencyBuilder WebPageOnLanguage(this ICacheDependencyBuilder builder, IWebPageFieldsSource page, ICacheReferenceService cachingLanguageService)
        {
            if (page.SystemFields.WebPageItemID > 0) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemID], cachingLanguageService.GetLanguageNameById(page.SystemFields.ContentItemCommonDataContentLanguageID));
            } else if (page.SystemFields.WebPageItemGUID != Guid.Empty) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemGUID], cachingLanguageService.GetLanguageNameById(page.SystemFields.ContentItemCommonDataContentLanguageID));
            } else if (!string.IsNullOrWhiteSpace(page.SystemFields.WebPageItemName)) {
                return builder.WebPagesOnLanguage([page.SystemFields.WebPageItemName], cachingLanguageService.GetLanguageNameById(page.SystemFields.ContentItemCommonDataContentLanguageID));
            }

            throw new CacheDependencyMissingDataException("No WebPageItemID, WebPageItemGUID, or WebPageItemName found, cannot cache, please correct query to include at least one of these.");
        }

        public static ICacheDependencyBuilder WebPageOnLanguage(this ICacheDependencyBuilder builder, int webPageItemID, string language) => WebPagesOnLanguage(builder, [webPageItemID], language);
        public static ICacheDependencyBuilder WebPagesOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<int> webPageItemIDs, string language)
        {
            return builder.AddKeys(webPageItemIDs.Select(webPageItemID => $"webpageitem|byid|{webPageItemID}|{language}"));
        }

        public static ICacheDependencyBuilder WebPageOnLanguage(this ICacheDependencyBuilder builder, Guid webPageItemGuid, string language) => WebPagesOnLanguage(builder, [webPageItemGuid], language);
        public static ICacheDependencyBuilder WebPagesOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<Guid> webPageItemGuids, string language)
        {
            return builder.AddKeys(webPageItemGuids.Select(webPageItemGuid => $"webpageitem|byguid|{webPageItemGuid}|{language}"));
        }

        public static ICacheDependencyBuilder WebPageOnLanguage(this ICacheDependencyBuilder builder, string webpageItemName, string language) => WebPagesOnLanguage(builder, [webpageItemName], language);
        public static ICacheDependencyBuilder WebPagesOnLanguage(this ICacheDependencyBuilder builder, IEnumerable<string> webpageItemNames, string language)
        {
            return builder.AddKeys(webpageItemNames.Select(webpageItemName => $"webpageitem|byname|{webpageItemName}|{language}"));
        }

        #endregion

        #region "General Objects"

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, int? id) => Object(builder, objectType, id.GetValueOrDefault());
        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, int id)
        {
            if (string.IsNullOrWhiteSpace(objectType) || id <= 0) {
                return builder;
            }

            builder.AddKey($"{objectType}|byid|{id}");

            return builder;
        }

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, Guid? guid) => Object(builder, objectType, guid.GetValueOrDefault());
        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, Guid guid)
        {
            if (guid == default) {
                return builder;
            }

            return builder.AddKey($"{objectType}|byguid|{guid}");
        }

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) {
                return builder;
            }

            return builder.AddKey($"{objectType}|byname|{name}");
        }

        public static ICacheDependencyBuilder Objects(this ICacheDependencyBuilder builder, IEnumerable<BaseInfo> objects)
        {
            return builder.AddKeys(objects.Select(o => $"{o.TypeInfo.ObjectType}|byid|{o.Generalized.ObjectID}"));
        }

        #endregion

        #region "Media Files"

        public static ICacheDependencyBuilder Media(this ICacheDependencyBuilder builder, Guid? mediaFileGUID) => Media(builder, mediaFileGUID.GetValueOrDefault());
        public static ICacheDependencyBuilder Media(this ICacheDependencyBuilder builder, Guid mediaFileGUID)
        {
            if (mediaFileGUID == default) {
                return builder;
            }

            return builder.AddKey($"mediafile|{mediaFileGUID}");
        }

        #endregion

        #region "Action Based  Helpers"

        public static ICacheDependencyBuilder Collection<T>(this ICacheDependencyBuilder builder, IEnumerable<T> items, Action<T, ICacheDependencyBuilder> action)
        {
            foreach (var item in items.Where(x => x != null)) {
                action(item, builder);
            }

            return builder;
        }
        public static ICacheDependencyBuilder Collection<T>(this ICacheDependencyBuilder builder, IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items.Where(x => x != null)) {
                action(item);
            }

            return builder;
        }


        public static ICacheDependencyBuilder ApplyDependenciesTo(this ICacheDependencyBuilder builder, Action<string> action)
        {
            foreach (string cacheKey in builder.GetKeys()) {
                action(cacheKey);
            }
            return builder;
        }
        public static ICacheDependencyBuilder ApplyAllDependenciesTo(this ICacheDependencyBuilder builder, Action<string[]> action)
        {
            action(builder.GetKeys().ToArray());
            return builder;
        }

        public static CMSCacheDependency GetCMSCacheDependency(this ICacheDependencyBuilder builder)
        {
            return CacheHelper.GetCacheDependency(builder.GetKeys().ToArray());
        }
        public static CMSCacheDependency GetCMSCacheDependency(this ICacheDependencyBuilder builder, IEnumerable<string> additionalDependencies)
        {
            return CacheHelper.GetCacheDependency(builder.GetKeys().Union(additionalDependencies).ToArray());
        }

        public static ICacheDependencyBuilder AppendDTOWithDependencies<T>(this ICacheDependencyBuilder builder, DTOWithDependencies<T> dtoWrapper)
        {
            return builder.AddKeys(dtoWrapper.AdditionalDependencies)
                .ContentItems(dtoWrapper.ContentItems)
                .WebPages(dtoWrapper.WebPageItems)
                .Objects(dtoWrapper.Objects);
        }

        #endregion
    }
}
