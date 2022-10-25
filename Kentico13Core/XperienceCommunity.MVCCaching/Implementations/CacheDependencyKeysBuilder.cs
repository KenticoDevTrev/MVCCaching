using CMS.Base;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XperienceCommunity.MVCCaching.Implementations
{
    /// <summary>
    /// Base implementation of the ICacheDependencyBuilder.  Most methods are defined in the ICacheDependencyBuilderExtensions.cs
    /// </summary>
    public class CacheDependencyBuilder : ICacheDependencyBuilder
    {
        private readonly HashSet<string> _cacheKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ISiteService _siteService;
        private bool AddToDependencyStore { get; set; }

        public CacheDependencyBuilder(ISiteService siteService)
        {
            _siteService = siteService;
            AddToDependencyStore = false;
        }
        public CacheDependencyBuilder(ISiteService siteService, ICacheDependenciesStore cacheDependenciesStore)
        {
            _siteService = siteService;
            _cacheDependenciesStore = cacheDependenciesStore;
            AddToDependencyStore = true;
        }

        public string SiteName()
        {
            return _siteService?.CurrentSite?.SiteName ?? "unknownsite";
        }

        public ISet<string> GetKeys() => _cacheKeys;

        private void Add(string key)
        {
            _cacheKeys.Add(key);
            if(AddToDependencyStore) { 
                _cacheDependenciesStore.Store(new string[] { key });
            }
        }

        private void UnionWith(IEnumerable<string> keys)
        {
            _cacheKeys.UnionWith(keys);
            if(AddToDependencyStore) { 
                _cacheDependenciesStore.Store(keys.ToArray());
            }
        }

        public ICacheDependencyBuilder AddKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this;
            }

            Add(key);

            return this;
        }
        public ICacheDependencyBuilder AddKeys(IEnumerable<string> keys)
        {
            UnionWith(keys);

            return this;
        }
    }
}
