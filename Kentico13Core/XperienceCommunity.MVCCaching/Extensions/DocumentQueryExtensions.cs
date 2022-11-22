using CMS.DocumentEngine;
using MVCCaching;

namespace Kentico.Content.Web.Mvc
{
    public static class DocumentQueryExtensions
    {
        /// <summary>
        /// Sets Culture, Combine with Default Culture, Published and Latest Version parameters on the query, useful for when you need to query pages on a differnet site/culture than the current.
        /// 
        /// For the Cache Key, make sure to also include ICacheRepositoryContext.ToCacheRepositoryContextNameIdentifier(), as well as any SiteName, which normally are automatically added when using IPageRetriever
        /// </summary>
        /// <param name="baseQuery">The Document Query</param>
        /// <param name="context">The ICacheRepositoryContext implementation</param>
        /// <returns></returns>
        public static DocumentQuery WithCulturePreviewModeContext(this DocumentQuery baseQuery, ICacheRepositoryContext context)
        {
            return baseQuery.Culture(context.CurrentCulture())
                .CombineWithDefaultCulture()
                .Published(!context.PreviewEnabled())
                .LatestVersion(context.PreviewEnabled());
        }

        /// <summary>
        /// Sets Culture, Combine with Default Culture, Published and Latest Version parameters on the query, useful for when you need to query pages on a differnet site/culture than the current.
        /// 
        /// For the Cache Key, make sure to also include ICacheRepositoryContext.ToCacheRepositoryContextNameIdentifier(), as well as any SiteName, which normally are automatically added when using IPageRetriever
        /// </summary>
        /// <param name="baseQuery">The Document Query</param>
        /// <param name="context">The ICacheRepositoryContext implementation</param>
        /// <returns></returns>
        public static DocumentQuery<TDocument> WithCulturePreviewModeContext<TDocument>(this DocumentQuery<TDocument> baseQuery, ICacheRepositoryContext context) where TDocument : TreeNode, new()
        {
            return baseQuery.Culture(context.CurrentCulture())
                .CombineWithDefaultCulture()
                .Published(!context.PreviewEnabled())
                .LatestVersion(context.PreviewEnabled());
        }

        /// <summary>
        /// Sets Culture, Combine with Default Culture, Published and Latest Version parameters on the query, useful for when you need to query pages on a differnet site/culture than the current.
        /// 
        /// For the Cache Key, make sure to also include ICacheRepositoryContext.ToCacheRepositoryContextNameIdentifier(), as well as any SiteName, which normally are automatically added when using IPageRetriever
        /// </summary>
        /// <param name="baseQuery">The MultiDocument Query</param>
        /// <param name="context">The ICacheRepositoryContext implementation</param>
        /// <returns></returns>
        public static MultiDocumentQuery WithCulturePreviewModeContext(this MultiDocumentQuery baseQuery, ICacheRepositoryContext context)
        {
            return baseQuery.Culture(context.CurrentCulture())
                .CombineWithDefaultCulture()
                .Published(!context.PreviewEnabled())
                .LatestVersion(context.PreviewEnabled());
        }
    }
}
