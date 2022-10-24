using CMS.Base;
using MVCCaching;
using MVCCaching.Base.Core.Interfaces;
using System.Threading.Tasks;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheDependencyKeysBuilderFactory : ICacheDependencyKeysBuilderFactory
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteService _siteService;

        public CacheDependencyKeysBuilderFactory(ICacheDependenciesStore cacheDependenciesStore,
            ISiteService siteService)
        {
            _cacheDependenciesStore = cacheDependenciesStore;
            _siteService = siteService;
        }

        public Task<ICacheDependencyKeysBuilder> CreateCacheDependencyKeysBuilder(bool addKeysToStore = true)
        {
            if(addKeysToStore) { 
                return Task.FromResult((ICacheDependencyKeysBuilder)new CacheDependencyKeysBuilder(_siteService, _cacheDependenciesStore));
            } else
            {
                return Task.FromResult((ICacheDependencyKeysBuilder)new CacheDependencyKeysBuilder(_siteService));
            }
        }
    }
}
