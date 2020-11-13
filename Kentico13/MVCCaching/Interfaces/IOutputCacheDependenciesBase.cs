using System.Collections.Generic;
namespace MVCCaching
{
    /// <summary>
    /// Base of the OutputCacheDependencies, the Kentico Implementation uses it's own IOutputCacheDependencies that inherits from this, but all Cache Dependency should contain this interface.
    /// </summary>
    public interface IOutputCacheDependenciesBase
    {
        /// <summary>
        /// Adds the custom Cache Dependency for a view.
        /// </summary>
        /// <param name="dependencyCacheKey">The Kentico Cache Dependency Key</param>
        void AddCacheItemDependency(string dependencyCacheKey);

        /// <summary>
        /// Adds the custom Cache Dependencies for a view.
        /// </summary>
        /// <param name="dependencyCacheKey">The Kentico Cache Dependency Keys</param>
        void AddCacheItemDependencies(IEnumerable<string> dependencyCacheKeys);
    }
}