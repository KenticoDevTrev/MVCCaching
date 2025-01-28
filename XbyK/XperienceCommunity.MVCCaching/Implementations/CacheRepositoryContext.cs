using CMS.Base;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using MVCCaching;
using System;
using System.Linq;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheRepositoryContext : ICacheRepositoryContext
    {
        private readonly IInfoProvider<ContentLanguageInfo> _contentLanguageInfoProvider;

        public CacheRepositoryContext(IHttpContextAccessor httpContextAccessor, IWebsiteChannelContext websiteChannelContext, IPreferredLanguageRetriever preferredLanguageRetriever, ISettingsService settingsService, IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider)
        {
            HttpContextAccessor = httpContextAccessor;
            WebsiteChannelContext = websiteChannelContext;
            PreferredLanguageRetriever = preferredLanguageRetriever;
            SettingsService = settingsService;
            _contentLanguageInfoProvider = contentLanguageInfoProvider;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public IWebsiteChannelContext WebsiteChannelContext { get; }
        public IPreferredLanguageRetriever PreferredLanguageRetriever { get; }
        public ISettingsService SettingsService { get; }

        public bool CacheEnabled()
        {
            return !PreviewEnabled();
        }
        public string CurrentCulture()
        {
            try
            {
                return PreferredLanguageRetriever.Get();
            } catch(Exception)
            {
                return _contentLanguageInfoProvider.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), 1).GetEnumerableTypedResult().FirstOrDefault()?.ContentLanguageName ??
                System.Globalization.CultureInfo.CurrentCulture.Name.Split("-")[0];
            }
        }

        public int CacheTimeInMinutes()
        {
            try
            {
                return SettingsService["CMSCacheMinutes"].ToInteger(0);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool PreviewEnabled()
        {
            try
            {
                return WebsiteChannelContext.IsPreview;
            } catch(Exception)
            {
                return false;
            }
        }

        public string GetCacheKey()
        {
            return $"context-{CurrentCulture()}-{PreviewEnabled()}";
        }
    }
}
