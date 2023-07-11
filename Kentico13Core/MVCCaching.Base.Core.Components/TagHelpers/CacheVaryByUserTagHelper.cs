using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MVCCaching.Internal;
using System.Text.Encodings.Web;

namespace MVCCaching.Base.Core.Components.TagHelpers
{
    [HtmlTargetElement("cache", Attributes = "vary-by-user")]
    public class CacheVaryByUserTagHelper : CacheTagHelper
    {
        private readonly ICacheTagHelperService _cacheTagHelperService;
        private readonly ICacheDependenciesStore _cacheDependenciesStore;

        public override int Order => 2;

        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            if (VaryByUser)
            {
                var keys = new[]
                {
                    _cacheTagHelperService.GetUserDependencyKey()
                };

                _cacheDependenciesStore.Store(keys);
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }


        public CacheVaryByUserTagHelper(CacheTagHelperMemoryCacheFactory factory, HtmlEncoder htmlEncoder, ICacheTagHelperService cacheTagHelperService, ICacheDependenciesStore cacheDependenciesStore) : base(factory, htmlEncoder)
        {
            _cacheTagHelperService = cacheTagHelperService;
            _cacheDependenciesStore = cacheDependenciesStore;
        }
    }
}