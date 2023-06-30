using CMS.ContactManagement;
using CMS.Helpers.Caching;
using CMS.Membership;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MVCCaching
{

    [HtmlTargetElement("cache", Attributes = TARGET_ATTRIBUTE)]
    public class CacheScopeTagHelper : CacheTagHelper
    {
        public const string TARGET_ATTRIBUTE = "scoped";
        public const string KEYS_ATTRIBUTE = "additional-keys";

        private readonly ICacheDependencyAdapter mCacheDependencyAdapter;
        private readonly ICacheDependenciesScope mCacheDependenciesScope;
        private readonly ICacheDependenciesStore mCacheDependenciesStore;
        private readonly ICacheRepositoryContext mCacheRepositoryContext;
        private readonly IMemoryCache mMemoryCache;

        public override int Order => 2;

        [HtmlAttributeName(KEYS_ATTRIBUTE)] public string[] AdditionalKeys { get; set; }

        [HtmlAttributeName("vary-by-contact")] public bool VaryByContact { get; set; }= false;

        /// <override />
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            mCacheDependenciesScope.Begin();

            if (!context.AllAttributes.ContainsName(nameof(Enabled))) 
            { 
                Enabled = mCacheRepositoryContext.CacheEnabled();
            }

            if (VaryByUser)
            {
                var keys = new[]
                {
                    $"{UserInfo.OBJECT_TYPE}|byid|{MembershipContext.AuthenticatedUser?.UserID ?? 0}"
                };

                mCacheDependenciesStore.Store(keys);
            }

            if (VaryByContact)
            {
                VaryByCookie = "CurrentContact";

                var keys = new[]
                {
                    $"{ContactInfo.OBJECT_TYPE}|byguid|{ContactManagementContext.CurrentContact?.ContactGUID ?? Guid.Empty}"
                };

                mCacheDependenciesStore.Store(keys);
            }

            if (!ExpiresAfter.HasValue)
            {
                var cacheTimeInMinutes = mCacheRepositoryContext.CacheTimeInMinutes();

                if (cacheTimeInMinutes > 0)
                { 
                    ExpiresAfter = TimeSpan.FromMinutes(cacheTimeInMinutes);
                }
            }
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (AdditionalKeys is not null)
            { 
                mCacheDependenciesStore.Store(AdditionalKeys);
            }

            var cacheKeys = mCacheDependenciesScope.End();

            if (!Enabled || cacheKeys is { Length: 0 })
            {
                return Task.CompletedTask;
            }

            var changeToken = mCacheDependencyAdapter.GetChangeToken(cacheKeys);

            var key = $"{Guid.NewGuid()}";
            mMemoryCache.Set(key, (object)null, changeToken);
            mMemoryCache.Remove(key);

            return Task.CompletedTask;
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
            mMemoryCache = memoryCache;
        }
    }
}
