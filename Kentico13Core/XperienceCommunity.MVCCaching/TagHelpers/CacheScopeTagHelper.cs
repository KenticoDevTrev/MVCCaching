using CMS.Helpers.Caching;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using MVCCaching;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace XperienceCommunity.MVCCaching.TagHelpers
{
    [HtmlTargetElement("cache-scope")]
    public class CacheScopeTagHelper : CacheTagHelper
    {
        private readonly IMemoryCache mMemoryCache;
        private readonly ICacheDependencyAdapter mCacheDependencyAdapter;
        private readonly ICacheDependenciesScope mCacheDependenciesScope;
        private readonly ICacheDependenciesStore mCacheDependenciesStore;
        private readonly ICacheRepositoryContext mCacheRepositoryContext;


        [HtmlAttributeName("additional-keys")]
        public string[] AdditionalKeys { get; set; }

        /// <inheritdoc />
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            mCacheDependenciesScope.Begin();

            Enabled = mCacheRepositoryContext.CacheEnabled();

            //sets cache time if not set by tag attribute to the Xperienece setting for system cache time in minutes
            var cacheTimeInMinutes = mCacheRepositoryContext.CacheTimeInMinutes();
            if (!ExpiresAfter.HasValue && cacheTimeInMinutes > 0)
                ExpiresAfter = TimeSpan.FromMinutes(cacheTimeInMinutes);
        }

        /// <inheritdoc />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            if (AdditionalKeys is not null)
                mCacheDependenciesStore.Store(AdditionalKeys);

            var cacheKeys = mCacheDependenciesScope.End();

            if (!Enabled || cacheKeys is { Length: 0 })
                return;

            var changeToken = mCacheDependencyAdapter.GetChangeToken(cacheKeys);

            var key = $"{Guid.NewGuid()}";
            mMemoryCache.Set(key, (object)null, changeToken);
            mMemoryCache.Remove(key);
        }



        public CacheScopeTagHelper(
            CacheTagHelperMemoryCacheFactory factory,
            HtmlEncoder htmlEncoder,
            ICacheDependencyAdapter cacheDependencyAdapter,
            ICacheDependenciesScope cacheDependenciesScope,
            ICacheDependenciesStore cacheDependenciesStore,
            ICacheRepositoryContext cacheRepositoryContext,
            IMemoryCache memoryCache)
            : base(factory, htmlEncoder)
        {
            mCacheDependencyAdapter = cacheDependencyAdapter;
            mCacheDependenciesScope = cacheDependenciesScope;
            mCacheDependenciesStore = cacheDependenciesStore;
            mCacheRepositoryContext = cacheRepositoryContext;

            //Using the shared MemoryCache from the factory cause size errors 
            mMemoryCache = memoryCache;
        }

    }
}
