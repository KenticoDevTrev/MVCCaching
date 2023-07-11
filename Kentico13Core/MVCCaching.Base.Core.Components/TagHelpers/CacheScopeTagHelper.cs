using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text.Encodings.Web;

namespace MVCCaching
{

    [HtmlTargetElement("cache", Attributes = TARGET_ATTRIBUTE)]
    public class CacheScopeTagHelper : CacheTagHelper
    {
        public const string TARGET_ATTRIBUTE = "scoped";

        private readonly ICacheDependenciesScope _cacheDependenciesScope;
        protected readonly ICacheRepositoryContext _cacheRepositoryContext;


        public override int Order => 1;


        /// <summary>
        /// Initializes the cache scope
        /// </summary>
        /// <param name="context"></param>
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            _cacheDependenciesScope.Begin();

            if (!context.AllAttributes.ContainsName(nameof(Enabled)))
            {
                Enabled = _cacheRepositoryContext.CacheEnabled();
            }


            if (!ExpiresAfter.HasValue)
            {
                var cacheTimeInMinutes = _cacheRepositoryContext.CacheTimeInMinutes();

                if (cacheTimeInMinutes > 0)
                {
                    ExpiresAfter = TimeSpan.FromMinutes(cacheTimeInMinutes);
                }
            }
        }


     

        public CacheScopeTagHelper(CacheTagHelperMemoryCacheFactory factory, HtmlEncoder htmlEncoder, ICacheRepositoryContext cacheRepositoryContext, ICacheDependenciesScope cacheDependenciesScope) 
            : base(factory, htmlEncoder)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
            _cacheDependenciesScope = cacheDependenciesScope;
        }
    }
}
