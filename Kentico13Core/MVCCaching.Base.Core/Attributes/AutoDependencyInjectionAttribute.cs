using System;

namespace MVCCaching
{
    /// <summary>
    /// Attribute to be placed on the implementation of an interface to automatically hook up DI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoDependencyInjectionAttribute : Attribute
    {
        public AutoDependencyInjectionAttribute(DependencyInjectionLifetime lifetime = DependencyInjectionLifetime.Scoped)
        {
            Lifetime = lifetime;
        }
        public AutoDependencyInjectionAttribute(DependencyInjectionLifetime lifetime, Type interfaceType)
        {
            Lifetime = lifetime;
            InterfaceType = interfaceType;
        }

        public DependencyInjectionLifetime Lifetime { get; }
        /// <summary>
        /// The interface it implements, may be null in which case it will implelment any interface it implements (that isn't ICacheKey)
        /// </summary>
        public Type InterfaceType { get; }
    }
}
