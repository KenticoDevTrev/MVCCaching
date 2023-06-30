using CMS.Core;
using Microsoft.Extensions.DependencyInjection;
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
                .AddScoped<ICacheDependencyBuilderFactory, CacheDependencyBuilderFactory>()
                .AddScoped<ICacheRepositoryContext, CacheRepositoryContext>()
                .AddSingleton<IContentItemMetadataProvider, ContentItemMetadataProvider>();
        }

        /// <summary>
        /// Register Dependency Injection on any classes with the [AutoDependencyInjection] attribute for assemblies that have the Kentico [assembly: AssemblyDiscoverable] attribute
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMVCCachingAutoDependencyInjectionByAttribute(this IServiceCollection services)
        {
            return InternalAddMVCCachingAutoDependencyInjectionByAttribute(services, AssemblyDiscoveryHelper.GetAssemblies(true));
        }

        /// <summary>
        /// Register Dependency Injection on any classes with the [AutoDependencyInjection] attribute for assemblies passed
        /// </summary>
        public static IServiceCollection AddMVCCachingAutoDependencyInjectionByAttribute(this IServiceCollection services, IEnumerable<Assembly> assemblies) => InternalAddMVCCachingAutoDependencyInjectionByAttribute(services, assemblies);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private static IServiceCollection InternalAddMVCCachingAutoDependencyInjectionByAttribute(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (Type type in assemblies.SelectMany(a => a.GetTypes()))
            {
                var attributes = type.GetCustomAttributes(typeof(AutoDependencyInjectionAttribute), true);
                if (attributes.Length > 0)
                {
                    var attr = (AutoDependencyInjectionAttribute)attributes[0];
                    Type implementedType = attr.InterfaceType;
                    // Grab any interface it implements that isn't ICacheKey
                    if (implementedType == null)
                    {
                        implementedType = type.GetInterfaces().Where(x => !x.Equals(typeof(ICacheKey)))
                                .FirstOrDefault();
                    }

                    if (implementedType != null)
                    {
                        switch (attr.Lifetime)
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

        /// <summary>
        /// Register Dependency Injection on any classes with the given suffixes.  Example "Repository" would set up DI for any class ending in Repository, like TabRepository
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMVCCachingAutoDependencyInjectionSuffixes(this IServiceCollection services, IEnumerable<string> suffixes)
        {
            return InternalAddMVCCachingAutoDependencyInjectionBySuffixes(services, suffixes, AssemblyDiscoveryHelper.GetAssemblies(true));
        }

        /// <summary>
        /// Register Dependency Injection on any classes with the [AutoDependencyInjection] attribute for assemblies passed
        /// </summary>
        public static IServiceCollection AddMVCCachingAutoDependencyInjectionBySuffixes(this IServiceCollection services, IEnumerable<string> suffixes, IEnumerable<Assembly> assemblies) => InternalAddMVCCachingAutoDependencyInjectionBySuffixes(services, suffixes, assemblies);


        private static IServiceCollection InternalAddMVCCachingAutoDependencyInjectionBySuffixes(IServiceCollection services, IEnumerable<string> suffixes, IEnumerable<Assembly> assemblies)
        {
            foreach (Type type in assemblies.SelectMany(a => a.GetTypes()
                .Where(t => (t.IsClass || t.IsAbstract) && suffixes.Any(s => t.Name.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
            ))
            {
                // logic borrowed from https://github.com/khellang/Scrutor/blob/b28d6ddf89c0b748cbc00c2a0b44dc4118e9cfa8/src/Scrutor/ReflectionExtensions.cs
                Type implementedType = type.GetInterfaces().Where(x => !x.Equals(typeof(ICacheKey)))
                    .FirstOrDefault();
                if (implementedType != null)
                {
                    services.AddScoped(implementedType, type);
                }
            }
            return services;
        }
    }
}
