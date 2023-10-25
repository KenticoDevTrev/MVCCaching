using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using MVCCaching.Internal;
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

        private readonly ICacheDependenciesScope _cacheDependenciesScope;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;
        private readonly ICacheTagHelperService _cacheTagHelperService;

        public CacheScopeTagHelper(
            HtmlEncoder htmlEncoder,
            ICacheDependenciesScope cacheDependenciesScope,
            ICacheDependenciesStore cacheDependenciesStore,
            ICacheRepositoryContext cacheRepositoryContext,
            ICacheTagHelperService cacheTagHelperService)
            : base(htmlEncoder)
        {
            _cacheDependenciesScope = cacheDependenciesScope;
            _cacheDependenciesStore = cacheDependenciesStore;
            _cacheRepositoryContext = cacheRepositoryContext;
            _cacheTagHelperService = cacheTagHelperService;
        }

        public override int Order => 100;

        [HtmlAttributeName(KEYS_ATTRIBUTE)] public string[] AdditionalKeys { get; set; } = Array.Empty<string>();

        [HtmlAttributeName("vary-by-contact")] public bool VaryByContact { get; set; } = false;

        /// <override />
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            _cacheDependenciesScope.Begin();

            if (VaryByUser)
            {
                var keys = new[]
                {
                    _cacheTagHelperService.GetUserDependencyKey()
                };

                _cacheDependenciesStore.Store(keys);
            }

            if (VaryByContact)
            {
                VaryByCookie = "CurrentContact";

                var keys = new[]
                {
                    _cacheTagHelperService.GetContactDependencyKey()
                };

                _cacheDependenciesStore.Store(keys);
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

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!context.AllAttributes.ContainsName(nameof(Enabled)))
            {
                Enabled = _cacheRepositoryContext.CacheEnabled();
            }

            if (AdditionalKeys is not null)
            {
                _cacheDependenciesStore.Store(AdditionalKeys);
            }

            var cacheKeys = _cacheDependenciesScope.End();

            if (!Enabled || cacheKeys is { Length: 0 })
            {
                return Task.CompletedTask;
            }

            _cacheTagHelperService.ChangeCacheTokenKeys(cacheKeys);

            return Task.CompletedTask;
        }


    }
}
