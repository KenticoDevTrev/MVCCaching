namespace MVCCaching
{
    public static class ICacheRepositoryContextExtensions
    {
        /// <summary>
        /// Gets a unique name identifier for the cache key with the current culture and preview enabled, useful if you leverage the DocumentQuery.WithCulturePreviewModeContext method.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ToCacheRepositoryContextNameIdentifier(this ICacheRepositoryContext context)
        {
            return $"context-{context.CurrentCulture()}-{context.PreviewEnabled()}";
        }
    }
}