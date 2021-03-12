using Autofac;
using CMS.DataEngine;
using CMS.SiteProvider;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Extras.DynamicProxy;
using Kentico.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MVCCaching.Interceptor;
using MVCCaching.Implementations;

namespace MVCCaching.Kentico
{



    /// <summary>
    /// Registers required implementations to the Autofac container and set the container as ASP.NET MVC dependency resolver
    /// </summary>
    public static class DependencyResolverConfig
    {
        public static void Register(ContainerBuilder builder, IEnumerable<Assembly> AssembliesToConfigure)
        {
            // Adds Kentico dependency logic for the Application
            ConfigureDependencyResolverForMvcApplication(builder, AssembliesToConfigure);
        }



        private static void ConfigureDependencyResolverForMvcApplication(ContainerBuilder builder, IEnumerable<Assembly> AssembliesToConfigure)
        {

            builder.RegisterType<CachingRepositoryContext>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RepoContext>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CachingRepositoryDecorator>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CacheDependenciesStore>().AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();

            foreach (Assembly AssemblyToConfigure in AssembliesToConfigure)
            {
                // Register repositories that use IRepository, passing culture and LatestVersionEnabled
                builder.RegisterAssemblyTypes(AssemblyToConfigure)
                       .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                       .AsImplementedInterfaces()
                       .EnableInterfaceInterceptors()
                       .InterceptedBy(typeof(CachingRepositoryDecorator))
                       .InstancePerLifetimeScope();

                // Register services that use IService
                builder.RegisterAssemblyTypes(AssemblyToConfigure)
                    .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();

                // Register cache implementation for ICacheHelper
                builder.RegisterAssemblyTypes(AssemblyToConfigure)
                    .Where(x => x.IsClass && !x.IsAbstract && typeof(ICacheHelper).IsAssignableFrom(x))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            // Register providers of additional information about content items
            builder.RegisterType<ContentItemMetadataProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}