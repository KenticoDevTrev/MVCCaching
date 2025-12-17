using CMS.Helpers;
using CMS.Websites.Routing;

namespace MVCCaching.Implementations
{
    public class CacheDependencyScopedBuilderFactory(ICacheDependenciesStore cacheDependenciesStore,
        IWebsiteChannelContext websiteChannelContext,
        ICacheDependencyBuilderFactory cacheDependencyBuilderFactory) : ICacheDependencyScopedBuilderFactory
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore = cacheDependenciesStore;
        private readonly IWebsiteChannelContext _websiteChannelContext = websiteChannelContext;
        private readonly ICacheDependencyBuilderFactory _cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;

        public ICacheDependencyScopedBuilder Create(bool addKeysToStore = true)
        {
            if(addKeysToStore) { 
                return new CacheDependencyScopedBuilder(!string.IsNullOrWhiteSpace(_websiteChannelContext.WebsiteChannelName) ? _websiteChannelContext.WebsiteChannelName : "unknownsite", _cacheDependencyBuilderFactory, _cacheDependenciesStore);
            } else
            {
                return new CacheDependencyScopedBuilder(!string.IsNullOrWhiteSpace(_websiteChannelContext.WebsiteChannelName) ? _websiteChannelContext.WebsiteChannelName : "unknownsite", _cacheDependencyBuilderFactory);
            }
        }

        public ICacheDependencyScopedBuilder Create(string specificSiteName, bool addKeysToStore = true)
        {
            if (addKeysToStore)
            {
                return new CacheDependencyScopedBuilder(specificSiteName, _cacheDependencyBuilderFactory, _cacheDependenciesStore);
            }
            else
            {
                return new CacheDependencyScopedBuilder(specificSiteName, _cacheDependencyBuilderFactory);
            }
        }
    }
}
