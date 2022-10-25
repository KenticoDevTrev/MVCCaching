using CMS.DocumentEngine;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kentico.Content.Web.Mvc
{
    public static class IPageCacheBuilderExtensions
    {
        /// <summary>
        /// Helper to simplify the configuration process.
        /// </summary>
        /// <typeparam name="TPageType"></typeparam>
        /// <param name="cs">The CacheBuilder</param>
        /// <param name="builder">The CacheDependencyKeysBuilder with it's dependencies</param>
        /// <param name="expirationMinutes">The Time for it to expire</param>
        /// <param name="keys">All keys, can accept IEnumerable, ICacheKey, and standard objects (which will return the .ToString(), which if you need a custom key for it then implement ICacheKey on the object class)</param>
        /// <returns></returns>
        public static IPageCacheBuilder<TPageType> Configure<TPageType>(this IPageCacheBuilder<TPageType> cs, ICacheDependencyBuilder builder, double expirationMinutes, params object[] keys) where TPageType : TreeNode, new()
        {
            string keyName = (keys != null && keys.Length > 0 ? string.Join("|", keys.Cast<object>().Select(x => x.ToCacheNameIdentifier())) : Guid.NewGuid().ToString());
            return cs.Key(keyName)
        .Dependencies((items, csbuilder) => builder.ApplyDependenciesTo(key => csbuilder.Custom(key)))
        .Expiration(TimeSpan.FromMinutes(expirationMinutes));
        }

        /// <summary>
        /// MAKE SURE TO HAVE DOCUMENTID RETURNING! And still add to the CacheDependencyKeysBuilder the returned DocumentIDs.
        /// 
        /// Helper to simplify the configuration process, this also takes the returned items and includes the returned items documentID in the data cache.  
        /// </summary>
        /// <typeparam name="TPageType"></typeparam>
        /// <param name="cs">The CacheBuilder</param>
        /// <param name="builder">The CacheDependencyKeysBuilder with it's dependencies</param>
        /// <param name="expirationMinutes">The Time for it to expire</param>
        /// <param name="keys">All keys, can accept IEnumerable, ICacheKey, and standard objects (which will return the .ToString(), which if you need a custom key for it then implement ICacheKey on the object class)</param>
        /// <returns></returns>
        public static IPageCacheBuilder<TPageType> ConfigureWithDocumentIDCaching<TPageType>(this IPageCacheBuilder<TPageType> cs, ICacheDependencyBuilder builder, double expirationMinutes, params object[] keys) where TPageType : TreeNode, new()
        {
            string keyName = (keys != null && keys.Length > 0 ? string.Join("|", keys.Cast<object>().Select(x => x.ToCacheNameIdentifier())) : Guid.NewGuid().ToString());
            return cs.Key(keyName)
        .Dependencies((items, csbuilder) => {
            foreach (var docID in items.Select(x => x.DocumentID))
            {
                builder.AddKey($"documentid|{docID}");
            }
            builder.ApplyDependenciesTo(key => csbuilder.Custom(key));
        })
        .Expiration(TimeSpan.FromMinutes(expirationMinutes));
        }

        /// <summary>
        /// Helper to simplify the configuration process.
        /// </summary>
        /// <typeparam name="TPageType"></typeparam>
        /// <param name="cs">The CacheBuilder</param>
        /// <param name="builder">The CacheDependencyKeysBuilder with it's dependencies</param>
        /// <param name="action">Action that allows you to add dependency keys based on the returned items as well.  Note these keys will keys will not be included in the CacheDependencyScope and will only apply to the PageRetriever data caching.  Make sure to add similar dependencies afterwards. </param>
        /// <param name="expirationMinutes">The Time for it to expire</param>
        /// <param name="keys">All keys, can accept IEnumerable, ICacheKey, and standard objects (which will return the .ToString(), which if you need a custom key for it then implement ICacheKey on the object class)</param>
        /// <returns></returns>
        public static IPageCacheBuilder<TPageType> Configure<TPageType>(this IPageCacheBuilder<TPageType> cs, ICacheDependencyBuilder builder, Action<IEnumerable<TPageType>, IPageCacheDependencyBuilder<TPageType>> action, double expirationMinutes, params object[] keys) where TPageType : TreeNode, new()
        {
            string keyName = (keys != null && keys.Length > 0 ? string.Join("|", keys.Cast<object>().Select(x => x.ToCacheNameIdentifier())) : Guid.NewGuid().ToString());
            return cs.Key(keyName)
        .Dependencies((items, csbuilder) => {
            action(items, csbuilder);
            builder.ApplyDependenciesTo(key => csbuilder.Custom(key));
        })
        .Expiration(TimeSpan.FromMinutes(expirationMinutes));
        }
    }
}
