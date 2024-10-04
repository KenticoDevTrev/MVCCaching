using System;

namespace MVCCaching
{
    public static class ICacheRepositoryContextExtensions
    {
        /// <summary>
        /// Gets a unique name identifier for the cache key with the current culture and preview enabled, useful if you leverage the DocumentQuery.WithCulturePreviewModeContext method.
        /// 
        /// If you wish to disable cache on Preview, make sure to set cs.Cached = _cacheRepositoryContext.CacheEnabled(); which will disable on preview.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Obsolete("ICacheRepositoryContext now inherits from ICacheKey, so you can use the .ToCacheNameIdentifier() now instead.")]
        public static string ToCacheRepositoryContextNameIdentifier(this ICacheRepositoryContext context)
        {
            return $"context-{context.CurrentCulture()}-{context.PreviewEnabled()}";
        }
    }
}