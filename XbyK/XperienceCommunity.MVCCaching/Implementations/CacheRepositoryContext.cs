using CMS.Base;
using CMS.Core;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using MVCCaching;
using System;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheRepositoryContext : ICacheRepositoryContext
    {

        public CacheRepositoryContext(IHttpContextAccessor httpContextAccessor, IWebsiteChannelContext websiteChannelContext, IPreferredLanguageRetriever preferredLanguageRetriever, ISettingsService settingsService)
        {
            HttpContextAccessor = httpContextAccessor;
            WebsiteChannelContext = websiteChannelContext;
            PreferredLanguageRetriever = preferredLanguageRetriever;
            SettingsService = settingsService;
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
                return System.Globalization.CultureInfo.CurrentCulture.Name;
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
    }
}
