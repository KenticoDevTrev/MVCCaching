using System.Threading.Tasks;

namespace MVCCaching.Base.Core.Interfaces
{
    public interface ICacheDependencyBuilderFactory
    {
        /// <summary>
        /// Gets a new instance of the ICacheDependencyBuilder.
        /// </summary>
        /// <param name="addKeysToStore">True by default, adds any keys to the ICacheStore, if false then will not (sometimes you want to use the builder just to build dependencies that should not be added to the store)</param>
        /// <returns></returns>
        ICacheDependencyBuilder Create(bool addKeysToStore = true);
    }
}
