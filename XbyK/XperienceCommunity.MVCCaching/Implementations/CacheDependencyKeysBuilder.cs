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
        private string SiteCodeName { get; set; }
        private bool AddToDependencyStore { get; set; }

        public CacheDependencyBuilder(string specificSiteCodeName)
        {
            SiteCodeName = specificSiteCodeName;
            AddToDependencyStore = false;
        }
        public CacheDependencyBuilder(string specificSiteCodeName, ICacheDependenciesStore cacheDependenciesStore)
        {
            SiteCodeName = specificSiteCodeName;
            _cacheDependenciesStore = cacheDependenciesStore;
            AddToDependencyStore = true;
        }

        public string SiteName()
        {
            return SiteCodeName ?? "unknownsite";
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
