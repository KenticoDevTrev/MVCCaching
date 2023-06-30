namespace MVCCaching
{
    public interface ICacheRepositoryContext
    {
        bool CacheEnabled();
        bool PreviewEnabled();
        string CurrentCulture();

        int CacheTimeInMinutes();

    }
}
