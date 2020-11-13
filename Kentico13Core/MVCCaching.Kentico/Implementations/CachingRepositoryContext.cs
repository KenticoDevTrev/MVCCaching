using Kentico.Web.Mvc;
using MVCCaching.Interceptor;
using System;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Http;
using CMS.DataEngine;
using CMS.SiteProvider;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace MVCCaching.Implementations
{
    public class CachingRepositoryContext : ICachingRepositoryContext
    {
        public CachingRepositoryContext(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            HttpContextAccessor = httpContextAccessor;
            Configuration = configuration;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public IConfiguration Configuration { get; }

        public bool CacheEnabled()
        {
            return !HttpContextAccessor.HttpContext.Kentico().Preview().Enabled;
        }

        public TimeSpan DefaultCacheItemDuration()
        {
            var value = Configuration["RepositoryCacheItemDuration"];

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds) && seconds > 0)
            {
                return TimeSpan.FromSeconds(seconds);
            }
            else
            {
                try
                {
                    return TimeSpan.FromMinutes(SettingsKeyInfoProvider.GetIntValue("CMSCacheMinutes", new SiteInfoIdentifier(SiteContext.CurrentSiteName)));
                }
                catch (Exception)
                {
                    return TimeSpan.Zero;
                }
            }
        }

    }
}
