using System;
using System.Threading;

using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace MVCCaching
{
    /// <summary>
    /// Tag helper for <c>cache-dependency-mvc</c> elements. Allows for specification of dependency cache keys within the <c>cache</c> element.
    /// </summary>
    [HtmlTargetElement("cache-dependency-mvc", Attributes = "cache-keys", TagStructure = TagStructure.WithoutEndTag)]
    public class CacheDependencyMvcTagHelper : TagHelper
    {
        private static IMemoryCache mMemoryCache;


        /// <summary>
        /// Gets or sets the array of dependency cache keys.
        /// </summary>
        public string[] CacheKeys { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the cache dependency is enabled. The default value is <c>true</c>.
        /// </summary>
        public bool Enabled { get; set; } = true;


        /// <summary>
        /// Gets the adapter for converting dependency cache keys to change tokens.
        /// </summary>
        protected virtual ICacheDependenciesAdapter CacheDependencyAdapter { get; }


        /// <summary>
        /// Gets or sets the memory cache used by this helper.
        /// </summary>
        protected static IMemoryCache MemoryCache
        {
            get
            {
                if (mMemoryCache == null)
                {
                    Interlocked.CompareExchange(ref mMemoryCache, new MemoryCache(new MemoryCacheOptions()), null);
                }
                return mMemoryCache;
            }
            set
            {
                mMemoryCache = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDependencyMvcTagHelper"/> class.
        /// </summary>
        /// <param name="cacheDependencyAdapter">Adapter for converting dependency cache keys to change tokens.</param>
        public CacheDependencyMvcTagHelper(ICacheDependenciesAdapter cacheDependencyAdapter)
        {
            CacheDependencyAdapter = cacheDependencyAdapter;

            MemoryCache = new MemoryCache(new MemoryCacheOptions());
        }


        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.SelfClosing;
            output.SuppressOutput();

            if (!Enabled || CacheKeys == null || CacheKeys.Length == 0)
            {
                return;
            }

            var changeToken = CacheDependencyAdapter.GetChangeToken(CacheKeys);

            var key = Guid.NewGuid().ToString();

            // Setting and removing a dummy cache item propagates the change token to parent MemoryCache scope (<cache> element scope)
            MemoryCache.Set<object>(key, null, changeToken);
            MemoryCache.Remove(key);
        }
    }
}