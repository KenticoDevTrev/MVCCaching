namespace MVCCaching.Internal
{
    public interface ICacheTagHelperService
    {
        /// <summary>
        /// Sets the cache keys for the tag helper
        /// </summary>
        /// <param name="cacheKeys"></param>
        void ChangeCacheTokenKeys(string[] cacheKeys);

        /// <summary>
        /// Gets the dependency key for the current user
        /// </summary>
        /// <returns></returns>
        string GetUserDependencyKey();

        /// <summary>
        /// Gets the dependency key for the current contact
        /// </summary>
        /// <returns></returns>
        string GetContactDependencyKey();
    }
}
