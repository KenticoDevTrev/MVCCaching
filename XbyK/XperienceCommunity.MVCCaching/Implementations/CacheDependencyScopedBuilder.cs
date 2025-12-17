using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCCaching.Implementations
{
    /// <summary>
    /// Wrapper for the Xperience CacheDependencyKey
    /// </summary>
    public class CacheDependencyScopedBuilder : ICacheDependencyScopedBuilder
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ICacheDependencyBuilderFactory _builderFactory;

        private readonly HashSet<string> _cacheKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private string SiteCodeName { get; set; }
        private bool AddToDependencyStore { get; set; }
        public CacheDependencyBuilder XperienceBuilder { get; set; }

        public CacheDependencyScopedBuilder(string specificSiteCodeName, ICacheDependencyBuilderFactory builderFactory)
        {
            SiteCodeName = specificSiteCodeName;
            _builderFactory = builderFactory;
            XperienceBuilder = builderFactory.Create();
            AddToDependencyStore = false;
        }
        public CacheDependencyScopedBuilder(string specificSiteCodeName, ICacheDependencyBuilderFactory builderFactory, ICacheDependenciesStore cacheDependenciesStore)
        {
            SiteCodeName = specificSiteCodeName;
            XperienceBuilder = builderFactory.Create();
            _cacheDependenciesStore = cacheDependenciesStore;
            AddToDependencyStore = true;
        }

        public string SiteName()
        {
            return SiteCodeName ?? "unknownsite";
        }

        public ISet<string> GetKeys() => XperienceBuilder.Build().CacheKeys.ToHashSet();

        private void Add(string key)
        {
            XperienceBuilder.AddDependency(key);
            _cacheKeys.Add(key);
            if (AddToDependencyStore) { 
                _cacheDependenciesStore.Store([key]);
            }
        }

        private void UnionWith(IEnumerable<string> keys)
        {
            var existingKeys = GetKeys();
            var allKeys = existingKeys.Union(keys, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);
            XperienceBuilder = _builderFactory.Create();
            foreach (var key in allKeys) {
                XperienceBuilder.AddDependency(key);
                _cacheKeys.Add(key);
            }
            if(AddToDependencyStore) { 
                _cacheDependenciesStore.Store([.. keys]);
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

        public void StoreUntrack()
        {
            // get difference of the current Builder and the internal keys and add to dependency store
            var existingKeys = GetKeys();
            var unTrackedKeys = _cacheKeys.Except(existingKeys, StringComparer.OrdinalIgnoreCase);
            if (AddToDependencyStore) {
                _cacheDependenciesStore.Store([.. unTrackedKeys]);
            }
        }
    }
}
