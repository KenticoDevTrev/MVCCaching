using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace MVCCaching
{
    [HtmlTargetElement("cache", Attributes = TARGET_ATTRIBUTE)]
    public class CacheVaryByPreviewTagHelper : CacheTagHelper
    {
       
        public const string TARGET_ATTRIBUTE = "vary-by-preview";
        private readonly ICacheRepositoryContext _cacheRepositoryContext;

        public CacheVaryByPreviewTagHelper(CacheTagHelperMemoryCacheFactory factory,
            HtmlEncoder htmlEncoder, ICacheRepositoryContext cacheRepositoryContext) : base(factory, htmlEncoder)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
        }

        public override int Order => 1;

        [HtmlAttributeName(TARGET_ATTRIBUTE)] public bool VaryByPreview { get; set; } = true;

        /// <override />
        public override void Init(TagHelperContext context)
        {
            base.Init(context);

            if (VaryByPreview)
            {
                if (!string.IsNullOrWhiteSpace(VaryBy))
                {
                    VaryBy += $"|Preview[{_cacheRepositoryContext.PreviewEnabled()}]";
                }
                else
                {
                    VaryBy = $"|Preview[{_cacheRepositoryContext.PreviewEnabled()}]";
                }
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
        }
    }
}
