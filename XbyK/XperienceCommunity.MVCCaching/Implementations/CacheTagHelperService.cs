using CMS.ContactManagement;
using CMS.Helpers.Caching;
using CMS.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MVCCaching.Internal;
using System;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheTagHelperService : ICacheTagHelperService
    {
        private readonly ICacheDependencyAdapter _cacheDependencyAdapter;
        private readonly IMemoryCache _memoryCache;

        public IHttpContextAccessor HttpContextAccessor { get; }

        public CacheTagHelperService(ICacheDependencyAdapter cacheDependencyAdapter,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _cacheDependencyAdapter = cacheDependencyAdapter;
            _memoryCache = memoryCache;
            HttpContextAccessor = httpContextAccessor;
        }

        public void ChangeCacheTokenKeys(string[] cacheKeys)
        {
            var changeToken = _cacheDependencyAdapter.GetChangeToken(cacheKeys);
            var key = $"{Guid.NewGuid()}";
            _memoryCache.Set(key, (object)null, changeToken);
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// Gets the dependency key for the current user
        /// </summary>
        /// <returns></returns>
        public string GetUserDependencyKey()
        {
            return $"{MemberInfo.OBJECT_TYPE}|byname|{HttpContextAccessor.HttpContext.User.Identity.Name}";
        }

        /// <summary>
        /// Gets the dependency key for the current contact
        /// </summary>
        /// <returns></returns>
        public string GetContactDependencyKey()
        {
            return $"{ContactInfo.OBJECT_TYPE}|byguid|{ContactManagementContext.CurrentContact?.ContactGUID ?? Guid.Empty}";
        }
    }
}
