namespace MVCCaching
{
    public interface ICacheRepositoryContext : ICacheKey
    {
        bool CacheEnabled();
        bool PreviewEnabled();
        string CurrentCulture();
        int CacheTimeInMinutes();
    }
}
