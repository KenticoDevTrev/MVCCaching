using Microsoft.AspNetCore.Razor.TagHelpers;
using MVCCaching.Internal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace MVCCaching.Base.Core.Components.TagHelpers
{
    [HtmlTargetElement("cache", Attributes = CONTACT_ATTRIBUTE)]
    public class CacheVaryByContactTagHelper : CacheTagHelper
    {
        private readonly ICacheDependenciesStore _cacheDependenciesStore;
        private readonly ICacheTagHelperService _cacheTagHelperService;


        public const string CONTACT_ATTRIBUTE = "vary-by-contact";
        [HtmlAttributeName(CONTACT_ATTRIBUTE)] public bool VaryByContact { get; set; } = false;

        public override int Order => 2;


        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            if (VaryByContact)
            {
                VaryByCookie = "CurrentContact";

                var keys = new[]
                {
                    _cacheTagHelperService.GetContactDependencyKey()
                };

                _cacheDependenciesStore.Store(keys);
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }

        public CacheVaryByContactTagHelper(CacheTagHelperMemoryCacheFactory factory, HtmlEncoder htmlEncoder, ICacheTagHelperService cacheTagHelperService, ICacheDependenciesStore cacheDependenciesStore) : base(factory, htmlEncoder)
        {
            _cacheTagHelperService = cacheTagHelperService;
            _cacheDependenciesStore = cacheDependenciesStore;
        }
    }
}
