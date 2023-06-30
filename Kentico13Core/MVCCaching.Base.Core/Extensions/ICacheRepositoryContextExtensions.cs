using System;

namespace MVCCaching
{
    public static class ICacheRepositoryContextExtensions
    {
        /// <summary>
        /// Gets a unique name identifier for the cache key with the current culture and preview enabled, useful if you leverage the DocumentQuery.WithCulturePreviewModeContext method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="disableCacheOnPreview">If true, and it is in preview mode, will add a random Guid to the name so it never caches.  This is important on data the user may need to preview during editing, but should be false for other data that isn't necessary.
        /// Note: this can have performance issues, as it will create new cache entries each preview, and should only be done on data that is NOT visible across the site (ex: Don't use this for header/footer stuff, but do use it for say a blog article display)
        /// </param>
        /// <returns></returns>
        public static string ToCacheRepositoryContextNameIdentifier(this ICacheRepositoryContext context, bool disableCacheOnPreview = false)
        {
            return $"context-{context.CurrentCulture()}-{context.PreviewEnabled()}{(context.PreviewEnabled() && disableCacheOnPreview ? "-"+Guid.NewGuid().ToString() : string.Empty )}";
        }
    }
}