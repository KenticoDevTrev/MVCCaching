using CMS.Base;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MVCCaching;
using System;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheRepositoryContext : ICacheRepositoryContext
    {
        private readonly ISiteService _siteService;

        public CacheRepositoryContext(IHttpContextAccessor httpContextAccessor, ISiteService siteService)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteService = siteService;
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
                    SettingsKeyInfoProvider.GetIntValue($"{_siteService.CurrentSite.SiteName}.CMSCacheMinutes");

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
