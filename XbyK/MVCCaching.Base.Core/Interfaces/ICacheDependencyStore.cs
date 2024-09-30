namespace MVCCaching
{
    public interface ICacheDependenciesStore
    {
        /// <summary>
        /// Stores the Cache Dependencies
        /// </summary>
        /// <param name="keys"></param>
        void Store(string[] keys);
    }
}
