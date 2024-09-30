using CMS.ContentEngine;
using MVCCaching;

namespace Kentico.Content.Web.Mvc
{
    public static class ContentQueryExtensions
    {
        /// <summary>
        /// Sets Culture, Combine with Default Culture.
        /// 
        /// For the Cache Key, make sure to also include ICacheRepositoryContext.ToCacheRepositoryContextNameIdentifier(), as well as any SiteName, which normally are automatically added when using IPageRetriever
        /// </summary>
        /// <param name="baseQuery">The ContentItemQueryBuilder Query</param>
        /// <param name="context">The ICacheRepositoryContext implementation</param>
        /// <returns>The Query Builder</returns>
        public static ContentItemQueryBuilder WithCultureContext(this ContentItemQueryBuilder baseQuery, ICacheRepositoryContext context)
        {
            return baseQuery
                .InLanguage(context.CurrentCulture(), useLanguageFallbacks: true);
        }

        /// <summary>
        /// Sets the Preview Mode onto your ContentQueryExecutionOptions.
        /// 
        /// For the Cache Key, make sure to also include ICacheRepositoryContext.ToCacheRepositoryContextNameIdentifier(), as well as any SiteName, which normally are automatically added when using IPageRetriever
        /// </summary>
        /// <param name="options">The ContentQueryExecutionOptions</param>
        /// <param name="context">The ICacheRepositoryContext implementation</param>
        /// <returns>The Query Execution Options</returns>
        public static ContentQueryExecutionOptions WithPreviewModeContext(this ContentQueryExecutionOptions options, ICacheRepositoryContext context) {
            options.ForPreview = context.PreviewEnabled();
            return options;
        }
    }
}
