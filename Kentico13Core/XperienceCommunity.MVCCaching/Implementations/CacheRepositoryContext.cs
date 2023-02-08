using CMS.Localization;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MVCCaching;
using System;
using CMS.DataEngine;
using CMS.SiteProvider;
using Kentico.PageBuilder.Web.Mvc;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheRepositoryContext : ICacheRepositoryContext
    {
        public CacheRepositoryContext(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor _httpContextAccessor { get; }

        public bool CacheEnabled()
        {
            return !PreviewEnabled();
        }
        public string CurrentCulture()
        {
            try
            {
                return PreviewEnabled() ? _httpContextAccessor.HttpContext.Kentico().Preview().CultureName : System.Globalization.CultureInfo.CurrentCulture.Name;
            } catch(Exception)
            {
                return System.Globalization.CultureInfo.CurrentCulture.Name;
            }
        }

        public int CacheTimeInMinutes()
        {
            try
            {
                var cacheTimeInMinutes =
                    SettingsKeyInfoProvider.GetIntValue($"{SiteContext.CurrentSiteName}.CMSCacheMinutes");

                return cacheTimeInMinutes;
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
                return _httpContextAccessor?.HttpContext?.Kentico()?.Preview()?.Enabled ?? false;
            } catch(Exception)
            {
                return false;
            }
        }
    }
}
