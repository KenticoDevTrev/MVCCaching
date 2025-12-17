namespace MVCCaching
{
    public interface ICacheDependencyScopedBuilderFactory
    {
        ICacheDependencyScopedBuilder Create(bool addKeysToStore = true);

        ICacheDependencyScopedBuilder Create(string specificSiteName, bool addKeysToStore = true);
    }
}
