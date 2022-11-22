using CMS.Base;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheDependencyBuilderFactory : ICacheDependencyBuilderFactory
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteService _siteService;

        public CacheDependencyBuilderFactory(ICacheDependenciesStore cacheDependenciesStore,
            ISiteService siteService)
        {
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteService = siteService;
        }

        public ICacheDependencyBuilder Create(bool addKeysToStore = true)
        {
            if(addKeysToStore) { 
                return new CacheDependencyBuilder(_siteService.CurrentSite?.SiteName ?? "unknownsite", _cacheDependenciesStore);
            } else
            {
                return new CacheDependencyBuilder(_siteService.CurrentSite?.SiteName ?? "unknownsite");
            }
        }

        public ICacheDependencyBuilder Create(string specificSiteName, bool addKeysToStore = true)
        {
            if (addKeysToStore)
            {
                return new CacheDependencyBuilder(specificSiteName, _cacheDependenciesStore);
            }
            else
            {
                return new CacheDependencyBuilder(specificSiteName);
            }
        }
    }
}
