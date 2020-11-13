using System;
using System.Collections.Generic;
using System.Text;

namespace MVCCaching.Interceptor
{
    public interface ICachingRepositoryContext
    {
        TimeSpan DefaultCacheItemDuration();
        bool CacheEnabled();
    }
}
