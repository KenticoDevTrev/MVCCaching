using CMS.Helpers.Caching;
using Kentico.Web.Mvc.Caching;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

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

            CacheKeys = MergeKeys();

            base.Process(context, output);
        }

        private string[] MergeKeys()
        {
            var firstArray = CacheKeys ?? Array.Empty<string>();
            var secondArray = _cacheDependenciesScope.End();

            var combinedArray = new string[firstArray.Length + secondArray.Length];
            for (var i = 0; i < combinedArray.Length; i++)
            {
                combinedArray[i] = i >= firstArray.Length ? secondArray[i - firstArray.Length] : firstArray[i];
            }
            return combinedArray;
        }

        public CacheDependencyKeyTagHelper(ICacheDependencyAdapter cacheDependencyAdapter, ICacheRepositoryContext cacheRepositoryContext, ICacheDependenciesScope cacheDependenciesScope) : base(cacheDependencyAdapter)
        {
            _cacheRepositoryContext = cacheRepositoryContext;
            _cacheDependenciesScope = cacheDependenciesScope;
        }
    }
}
