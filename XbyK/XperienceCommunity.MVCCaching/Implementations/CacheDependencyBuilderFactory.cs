using CMS.Websites.Routing;
using MVCCaching;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheDependencyBuilderFactory : ICacheDependencyBuilderFactory
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly IWebsiteChannelContext _websiteChannelContext;

        public CacheDependencyBuilderFactory(ICacheDependenciesStore cacheDependenciesStore,
            IWebsiteChannelContext websiteChannelContext)
        {
            _cacheDependenciesStore = cacheDependenciesStore;
            _websiteChannelContext = websiteChannelContext;
        }

        public ICacheDependencyBuilder Create(bool addKeysToStore = true)
        {
            if(addKeysToStore) { 
                return new CacheDependencyBuilder(!string.IsNullOrWhiteSpace(_websiteChannelContext.WebsiteChannelName) ? _websiteChannelContext.WebsiteChannelName : "unknownsite", _cacheDependenciesStore);
            } else
            {
                return new CacheDependencyBuilder(!string.IsNullOrWhiteSpace(_websiteChannelContext.WebsiteChannelName) ? _websiteChannelContext.WebsiteChannelName : "unknownsite");
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
