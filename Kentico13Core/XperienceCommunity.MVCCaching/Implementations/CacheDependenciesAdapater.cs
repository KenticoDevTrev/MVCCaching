using CMS.Helpers.Caching;
using Microsoft.Extensions.Primitives;
using MVCCaching;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheDependenciesAdapater : ICacheDependenciesAdapter
    {
        private readonly ICacheDependencyAdapter _cacheDependencyAdapter;

        public CacheDependenciesAdapater(ICacheDependencyAdapter cacheDependencyAdapter)
        {
            _cacheDependencyAdapter = cacheDependencyAdapter;
        }

        public IChangeToken GetChangeToken(string[] dependencyCacheKeys)
        {
            return _cacheDependencyAdapter.GetChangeToken(dependencyCacheKeys); 
        }
    }
}
