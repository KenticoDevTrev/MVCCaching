﻿using System.Collections.Generic;

namespace MVCCaching
{
    /// <summary>
    /// Cache Dependency Keys Builder.  Use Extension Methods to expand functionality.
    /// </summary>
    public interface ICacheDependencyKeysBuilder
    {
        /// <summary>
        /// Gets the current Keys
        /// </summary>
        /// <returns></returns>
        ISet<string> GetKeys();

        /// <summary>
        /// Adds a single custom key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ICacheDependencyKeysBuilder CustomKey(string key);

        /// <summary>
        /// Adds an array of Custom Keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        ICacheDependencyKeysBuilder CustomKeys(IEnumerable<string> keys);

        /// <summary>
        /// Used mainly for extensions to add any SiteName parameter to the dependency
        /// </summary>
        /// <returns></returns>
        string SiteName();
    }
}
