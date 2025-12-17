using CMS.Helpers;

namespace MVCCaching
{
    public interface ICacheDependencyScopedBuilder : ICacheDependencyBuilder
    {
        CacheDependencyBuilder XperienceBuilder { get; }

        /// <summary>
        /// If using the CacheDependencyBuilder's Fluent API to add keys, use the StoreUntracked to add any to the scope that were generated.
        /// </summary>
        void StoreUntrack();
    }
}
