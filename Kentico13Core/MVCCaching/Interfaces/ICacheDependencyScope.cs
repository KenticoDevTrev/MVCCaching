using System.Collections.Generic;

namespace MVCCaching.Base.Core.Interfaces
{
    /// <summary>
    /// Used to begin a new Cache dependency scope (starts collection of cache dependencies) and then End (which retrieves all cache dependencies set within there)
    /// </summary>
    public interface ICacheDependenciesScope
    {
        /// <summary>
        /// Begins a new Scope point.
        /// </summary>
        void Begin();

        /// <summary>
        /// Returns all Cache dependencies set via the ICacheDependencyStore.Store that occurred between the matching Begin and this call.
        /// </summary>
        /// <returns></returns>
        string[] End();
    }
}