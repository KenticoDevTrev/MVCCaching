using CMS.Localization;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using System.Web;

namespace MVCCaching.Implementations
{
    public class RepoContext : IRepoContext
    {
        public RepoContext()
        {
        }


        public string CurrentCulture()
        {
            return PreviewEnabled() ? HttpContext.Current.Kentico().Preview().CultureName : LocalizationContext.CurrentCulture.CultureCode;
        }

        public bool PreviewEnabled()
        {
            return HttpContext.Current.Kentico().Preview().Enabled;
        }
    }
}
