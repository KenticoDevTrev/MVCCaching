using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CachingReferenceService(IProgressiveCache progressiveCache,
                                         IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
                                         IWebsiteChannelContext websiteChannelContext,
                                         IInfoProvider<ChannelInfo> channelInfoProvider,
                                         ILinkedItemsDependencyAsyncRetriever linkedItemsDependencyAsyncRetriever,
                                         IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyAsyncRetriever) : ICachingReferenceService
    {
        public IProgressiveCache ProgressiveCache { get; } = progressiveCache;
        public IInfoProvider<ContentLanguageInfo> ContentLanguageInfoProvider { get; } = contentLanguageInfoProvider;
        public IWebsiteChannelContext WebsiteChannelContext { get; } = websiteChannelContext;
        public IInfoProvider<ChannelInfo> ChannelInfoProvider { get; } = channelInfoProvider;
        public ILinkedItemsDependencyAsyncRetriever LinkedItemsDependencyAsyncRetriever { get; } = linkedItemsDependencyAsyncRetriever;
        public IWebPageLinkedItemsDependencyAsyncRetriever WebPageLinkedItemsDependencyAsyncRetriever { get; } = webPageLinkedItemsDependencyAsyncRetriever;

        public string GetChannelNameByChannelId(int channelId)
        {
            return ProgressiveCache.Load(cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"{ChannelInfo.OBJECT_TYPE}|all");
                }

                return ChannelInfoProvider.Get()
                    .Columns(nameof(ChannelInfo.ChannelName), nameof(ChannelInfo.ChannelID))
                    .GetEnumerableTypedResult()
                    .ToDictionary(key => key.ChannelID, value => value.ChannelName);
            }, new CacheSettings(1440, "GetChannelNameByChannelId_MVCCaching"))
                .TryGetValue(channelId, out var channelName) ? channelName : WebsiteChannelContext.WebsiteChannelName;
        }

        public string GetChannelNameByWebsiteChannelId(int websiteChannelId)
        {
            return ProgressiveCache.Load(cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"{WebsiteChannelInfo.OBJECT_TYPE}|byid|{websiteChannelId}");
                }

                return ChannelInfoProvider.Get()
                    .Source(x => x.InnerJoin<WebsiteChannelInfo>(nameof(ChannelInfo.ChannelID), nameof(WebsiteChannelInfo.WebsiteChannelChannelID)))
                    .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelID), websiteChannelId)
                    .Columns(nameof(ChannelInfo.ChannelName))
                    .GetEnumerableTypedResult()
                    .FirstOrDefault()?.ChannelName ?? WebsiteChannelContext.WebsiteChannelName; // Should never be empty as Website Channel's Primary Language ID is a required non null foreign key
            }, new CacheSettings(1440, "GetChannelNameByWebsiteChannelId_MVCCaching", websiteChannelId));
        }

        public string GetCurrentWebChannelName() => WebsiteChannelContext.WebsiteChannelName;

        public string GetDefaultLanguageName(int websiteChannelId)
        {
            return ProgressiveCache.Load(cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"{WebsiteChannelInfo.OBJECT_TYPE}|byid|{websiteChannelId}");
                }

                return ContentLanguageInfoProvider.Get()
                    .Source(x => x.LeftJoin<WebsiteChannelInfo>(nameof(ContentLanguageInfo.ContentLanguageID), nameof(WebsiteChannelInfo.WebsiteChannelPrimaryContentLanguageID)))
                    .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelID), websiteChannelId)
                    .Columns(nameof(ContentLanguageInfo.ContentLanguageName))
                    .GetEnumerableTypedResult()
                    .FirstOrDefault()?.ContentLanguageName ?? String.Empty; // Should never be empty as Website Channel's Primary Language ID is a required non null foreign key
            }, new CacheSettings(1440, "GetDefaultLanguageName_MVCCaching", websiteChannelId));
        }

        public string GetLanguageNameById(int languageId)
        {
            return ProgressiveCache.Load(cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency($"{ContentLanguageInfo.OBJECT_TYPE}|all");
                }

                return ContentLanguageInfoProvider.Get()
                    .Columns(nameof(ContentLanguageInfo.ContentLanguageID), nameof(ContentLanguageInfo.ContentLanguageName))
                    .GetEnumerableTypedResult()
                    .ToDictionary(key => key.ContentLanguageID, value => value.ContentLanguageName);
            }, new CacheSettings(1440, "GetLanguageNameById_MVCCaching"))
            .TryGetValue(languageId, out var name) ? name : GetDefaultLanguageName(WebsiteChannelContext.WebsiteChannelID);
        }

        public async Task<IEnumerable<string>> GetLinkedItemDependenciesByContentItemsCachedAsync(IEnumerable<int> contentItemIDs, int maxLevel = 5, int cacheDuration = 360, CancellationToken cancellationToken = default)
        {
            var linkedItemDependency = new List<string>() { $"{ContentItemCommonDataInfo.OBJECT_TYPE}|all" };
            var dependencies = await ProgressiveCache.LoadAsync(async cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(linkedItemDependency);
                }

                return await LinkedItemsDependencyAsyncRetriever.Get(contentItemIDs, maxLevel, cancellationToken);
            }, new CacheSettings(cacheDuration, "GetLinkedItemDependenciesByContentItemsCachedAsync_MVCCaching", string.Join(",", contentItemIDs)));
            return dependencies.Union(linkedItemDependency);
        }

        public async Task<IEnumerable<string>> GetLinkedItemDependenciesByWebPageItemsCachedAsync(IEnumerable<int> webpageItemIds, int maxLevel = 5, int cacheDuration = 360, CancellationToken cancellationToken = default)
        {
            var linkedItemDependency = new List<string>() { $"{ContentItemCommonDataInfo.OBJECT_TYPE}|all" };
            var dependencies = await ProgressiveCache.LoadAsync(async cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency(linkedItemDependency);
                }

                return await WebPageLinkedItemsDependencyAsyncRetriever.Get(webpageItemIds, maxLevel, cancellationToken);
            }, new CacheSettings(cacheDuration, "GetLinkedItemDependenciesByContentItemsCachedAsync_MVCCaching", string.Join(",", webpageItemIds)));
            return dependencies.Union(linkedItemDependency);
        }
    }
}
