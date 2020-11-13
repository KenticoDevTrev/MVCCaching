using CMS.Localization;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using MVCCaching.Interceptor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVCCaching.Implementations
{
    public class RepoContext : IRepoContext
    {
        public RepoContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public string CurrentCulture()
        {
            return PreviewEnabled() ? HttpContextAccessor.HttpContext.Kentico().Preview().CultureName : LocalizationContext.CurrentCulture.CultureCode;
        }

        public bool PreviewEnabled()
        {
            return HttpContextAccessor.HttpContext.Kentico().Preview().Enabled;
        }
    }
}
