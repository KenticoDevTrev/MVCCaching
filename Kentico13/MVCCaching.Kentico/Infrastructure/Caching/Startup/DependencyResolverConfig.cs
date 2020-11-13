using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.Mvc;

using CMS.DataEngine;
using CMS.SiteProvider;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using MVCCaching.Implementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MVCCaching.Kentico
{
    /// <summary>
    /// Registers required implementations to the Autofac container and set the container as ASP.NET MVC dependency resolver
    /// </summary>
    public static class DependencyResolverConfig
    {
        public static void Register(ContainerBuilder builder, IEnumerable<Assembly> Assemblies)
        {
            // Adds Kentico dependency logic for the Application
            ConfigureDependencyResolverForMvcApplication(builder, Assemblies);

            AttachCMSDependencyResolver(builder);
        }

        private static void ConfigureDependencyResolverForMvcApplication(ContainerBuilder builder, IEnumerable<Assembly> Assemblies)
        {
            // Enable property injection in view pages
            builder.RegisterSource(new ViewRegistrationSource());

            // Register web abstraction classes
            builder.RegisterModule<AutofacWebTypesModule>();

            builder.RegisterType<RepoContext>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

            foreach (var AssemblyItem in Assemblies) { 
            // Register controllers
            builder.RegisterControllers(AssemblyItem);

            // Register repositories that use IRepository, passing culture and LatestVersionEnabled
            builder.RegisterAssemblyTypes(AssemblyItem)
                   .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                   .AsImplementedInterfaces()
                   .WithParameter((parameter, context) => parameter.Name == "cultureName", (parameter, context) => CultureInfo.CurrentUICulture.Name)
                   .WithParameter((parameter, context) => parameter.Name == "latestVersionEnabled", (parameter, context) => IsPreviewEnabled())
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(CachingRepositoryDecorator))
                   .InstancePerLifetimeScope();

            // Register services that use IService
            builder.RegisterAssemblyTypes(AssemblyItem)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Register services that use IOutputCacheDependencies
            builder.RegisterAssemblyTypes(AssemblyItem)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IOutputCacheDependencies).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

                // Register cache implementation for ICacheHelper
                builder.RegisterAssemblyTypes(AssemblyItem)
                    .Where(x => x.IsClass && !x.IsAbstract && typeof(ICacheHelper).IsAssignableFrom(x))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
            // Register providers of additional information about content items
            builder.RegisterType<ContentItemMetadataProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();

            // Register caching decorator for repositories
            builder.Register(context => new CachingRepositoryDecorator(GetCacheItemDuration(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled(), context.Resolve<IOutputCacheDependencies>()))
                .InstancePerLifetimeScope();

            // Enable declaration of output cache dependencies in controllers
            builder.Register(context => new OutputCacheDependencies(context.Resolve<HttpResponseBase>(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled()))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Configures Autofac container to use CMS dependency resolver in case it cannot resolve a dependency.  I believe this also hooks up Kentico's Services which include Cache clearing dependency detection
        /// </summary>
        private static void AttachCMSDependencyResolver(ContainerBuilder builder)
        {
            builder.RegisterSource(new CMSRegistrationSource());
        }

        /// <summary>
        /// Helper to detect if Cache should be enabled based on the Preview
        /// </summary>
        /// <returns></returns>
        private static bool IsCacheEnabled()
        {
            return !IsPreviewEnabled();
        }

        /// <summary>
        /// Helper to detect if Preview is enabled for the current request
        /// </summary>
        /// <returns></returns>
        private static bool IsPreviewEnabled()
        {
            return HttpContext.Current.Kentico().Preview().Enabled;
        }

        /// <summary>
        /// Gets the Cache Minute either from the AppSetting, or from the Cache Minutes Setting for data calls
        /// </summary>
        /// <returns></returns>
        private static TimeSpan GetCacheItemDuration()
        {
            var value = ConfigurationManager.AppSettings["RepositoryCacheItemDuration"];

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds) && seconds > 0)
            {
                return TimeSpan.FromSeconds(seconds);
            }
            else
            {
                try
                {
                    return TimeSpan.FromMinutes(SettingsKeyInfoProvider.GetIntValue("CMSCacheMinutes", new SiteInfoIdentifier(SiteContext.CurrentSiteName)));
                }
                catch (Exception)
                {
                    return TimeSpan.Zero;
                }
            }
        }
    }
}