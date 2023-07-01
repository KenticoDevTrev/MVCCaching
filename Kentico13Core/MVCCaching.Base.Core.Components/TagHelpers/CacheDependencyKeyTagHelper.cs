using CMS.Helpers.Caching;
using Kentico.Web.Mvc.Caching;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace MVCCaching.Base.Core.Components.TagHelpers
{
    [HtmlTargetElement("cache-dependency",
        Attributes = TARGET_ATTRIBUTE, 
        ParentTag = "cache", 
        TagStructure = TagStructure.WithoutEndTag)]
    public class CacheDependencyKeyTagHelper : CacheDependencyTagHelper
    {
        public const string TARGET_ATTRIBUTE = "scoped";

        private readonly ICacheDependenciesScope _cacheDependenciesScope;
        private readonly ICacheRepositoryContext _cacheRepositoryContext;

        public override int Order => 1;

        public override void Init(TagHelperContext context)
        {
            if (!context.AllAttributes.ContainsName(nameof(Enabled)))
                Enabled = _cacheRepositoryContext.CacheEnabled();
            
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(output);

            CacheKeys = CacheKeys.Union(_cacheDependenciesScope.End()).ToArray();

            base.Process(context, output);
        }

        public CacheDependencyKeyTagHelper(ICacheDependencyAdapter cacheDependencyAdapter, ICacheRepositoryContext cacheRepositoryContext, ICacheDependenciesScope cacheDependenciesScope) : base(cacheDependencyAdapter)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
            _cacheDependenciesScope = cacheDependenciesScope;
        }
    }
}
