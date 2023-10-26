using Microsoft.Extensions.Primitives;
using System;

namespace MVCCaching
{
    /// <summary>
    /// Adapts dependency cache keys to change tokens.
    /// </summary>
    /// <seealso cref="IMemoryCache"/>
    public interface ICacheDependenciesAdapter
    {
        /// <summary>
        /// Gets a change token for an array of dependency cache keys.
        /// </summary>
        /// <param name="dependencyCacheKeys">Dependency cache keys to watch.</param>
        /// <returns>Returns an <see cref="IChangeToken"/> for watching changes of the <paramref name="dependencyCacheKeys"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencyCacheKeys"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="dependencyCacheKeys"/> is an empty array.</exception>
        IChangeToken GetChangeToken(string[] dependencyCacheKeys);
    }
}
