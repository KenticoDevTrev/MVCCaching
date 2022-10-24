using CMS.Core;
using Microsoft.Extensions.DependencyInjection;
using MVCCaching.Base.Core.Interfaces;
using MVCCaching.Kentico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XperienceCommunity.MVCCaching.Implementations;

namespace MVCCaching
{
    public static class IServiceBuilderExtensions
    {
        public static IServiceCollection AddMVCCaching(this IServiceCollection services)
        {
            return services
                .AddScoped<CacheDependenciesStoreAndScope>()
                .AddScoped<ICacheDependenciesStore>(x => x.GetRequiredService<CacheDependenciesStoreAndScope>())
                .AddScoped<ICacheDependenciesScope>(x => x.GetRequiredService<CacheDependenciesStoreAndScope>())
                .AddScoped<ICacheDependencyKeysBuilderFactory, CacheDependencyKeysBuilderFactory>()
                .AddScoped<ICacheRepositoryContext, CacheRepositoryContext>()
                .AddSingleton<IContentItemMetadataProvider, ContentItemMetadataProvider>();
        }

        /// <summary>
        /// Register Dependency Injection on any classes with the [AutoDependencyInjection] attribute for assemblies that have the Kentico [assembly: AssemblyDiscoverable] attribute
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMVCCachingAutoDependencyInjection(this IServiceCollection services)
        {
            return InternalAddMVCCachingAutoDependencyInjection(services, AssemblyDiscoveryHelper.GetAssemblies(true));
        }

        /// <summary>
        /// Register Dependency Injection on any classes with the [AutoDependencyInjection] attribute for assemblies passed
        /// </summary>
        public static IServiceCollection AddMVCCachingAutoDependencyInjection(this IServiceCollection services, IEnumerable<Assembly> assemblies) => InternalAddMVCCachingAutoDependencyInjection(services, assemblies);

        private static IServiceCollection InternalAddMVCCachingAutoDependencyInjection(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (Type type in assemblies.SelectMany(a => a.GetTypes()))
            {
                var attributes = type.GetCustomAttributes(typeof(AutoDependencyInjectionAttribute), true);
                if (attributes.Length > 0)
                {
                    var attr = (AutoDependencyInjectionAttribute)attributes[0];
                    Type implementedType = attr.InterfaceType;
                    // Grab any interface it implements that isn't ICacheKey
                    if(implementedType == null)
                    {
                        implementedType = type.GetInterfaces().Where(x => !x.Equals(typeof(ICacheKey))).FirstOrDefault();
                    }

                    if(implementedType != null)
                    {
                        switch(attr.Lifetime)
                        {
                            default:
                            case DependencyInjectionLifetime.Singleton:
                                services.AddSingleton(implementedType, type);
                                break;
                            case DependencyInjectionLifetime.Scoped:
                                services.AddScoped(implementedType, type);
                                break;
                            case DependencyInjectionLifetime.Transient:
                                services.AddTransient(implementedType, type);
                                break;
                        }
                    }
                }
            }
            return services;
        }
    }
}
