using CMS.Base;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;
using System.Threading.Tasks;

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
                return (ICacheDependencyBuilder)new CacheDependencyBuilder(_siteService, _cacheDependenciesStore);
            } else
            {
                return (ICacheDependencyBuilder)new CacheDependencyBuilder(_siteService);
            }
        }
    }
}
