using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace MVCCaching
{
    [HtmlTargetElement("cache", Attributes = TARGET_ATTRIBUTE)]
    public class CacheVaryByPreviewTagHelper : CacheTagHelper
    {
        private readonly ICacheRepositoryContext _cacheRepositoryContext;

        public const string TARGET_ATTRIBUTE = "vary-by-preview";

        public override int Order => 2;

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

        public CacheVaryByPreviewTagHelper(CacheTagHelperMemoryCacheFactory factory, HtmlEncoder htmlEncoder, ICacheRepositoryContext cacheRepositoryContext) : base(factory, htmlEncoder)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
        }
    }
}
